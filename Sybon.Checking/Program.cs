using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using Sybon.Common;

namespace Sybon.Checking
{
    public static class Program
    {
        public static void Main(string[] args)
        {   
            var checkingService = new CheckingService(BuildWebHost);
            StartupHandler<CheckingService>.Handle(checkingService, args);
        }
        
        // It's a bit silly but it needs to belong here
        // Otherwise EF migrations don't want to work.
        // ReSharper disable once MemberCanBePrivate.Global
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .UseUrls("http://0.0.0.0:8194")
                .Build();
    }
}