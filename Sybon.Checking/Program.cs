using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using DasMulli.Win32.ServiceUtils;
using NLog.Web;

namespace Sybon.Checking
{
    public static class Program
    {
        private const string RunAsServiceFlag = "--run-as-service";
        private const string RegisterServiceFlag = "--register-service";
        private const string UnregisterServiceFlag = "--unregister-service";
        private const string ServiceName = "Sybon.Checking";
        private const string ServiceDisplayName = "Sybon.Checking web api";
        private const string ServiceDescription = "Sybon.Checking web api";
        public static void Main(string[] args)
        {   
            var myService = new MyService(BuildWebHost);

            if (args.Contains(RegisterServiceFlag))
            {
                new Win32ServiceManager().CreateService(
                    ServiceName,
                    ServiceDisplayName,
                    ServiceDescription,
                    Process.GetCurrentProcess().MainModule.FileName + " --run-as-service",
                    Win32ServiceCredentials.LocalSystem,
                    true,
                    true
                );
                Console.WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
            }
            else if (args.Contains(UnregisterServiceFlag))
            {
                new Win32ServiceManager().DeleteService(ServiceName);
                Console.WriteLine($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
            }
            else if (args.Contains(RunAsServiceFlag))
            {
                Console.WriteLine(Directory.GetCurrentDirectory());
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                var serviceHost = new Win32ServiceHost(myService);
                serviceHost.Run();
            }
            else
            {
                myService.Start(new string[0], () => { });
                Console.WriteLine("Running interactively, press enter to stop.");
                Console.ReadLine();
                myService.Stop();
            }
        }
        
        // It's a bit silly but it needs to belong here
        // Otherwise EF migrations don't want to work.
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .UseUrls("http://0.0.0.0:8194")
                .Build();
    }

    internal class MyService : IWin32Service
    {
        private IWebHost _app;
        private readonly Func<string[], IWebHost> _buildWebHostFunc;

        public MyService(Func<string[], IWebHost> buildWebHostFunc)
        {
            _buildWebHostFunc = buildWebHostFunc;
        }

        public string ServiceName => "Sybon.Checking";

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                _app = _buildWebHostFunc(startupArguments);
                _app.Start();
            }
            catch (Exception e)
            {
                //NLog: catch setup errors
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
        }

        public void Stop()
        {
            _app.StopAsync().Wait();
        }
    }
}