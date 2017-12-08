using System;
using System.Collections.Generic;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bunsan.Broker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Sybon.Archive.Client.Api;
using Sybon.Checking.Controllers;
using Sybon.Checking.Repositories.CompilersRepository;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Checking.Repositories.SubmitsRepository;
using Sybon.Checking.Services.CompilersService;
using Sybon.Checking.Services.SubmitCallbackService;
using Sybon.Checking.Services.SubmitResultService;
using Sybon.Checking.Services.SubmitService;
using Sybon.Common;
using AccountApi = Sybon.Auth.Client.Api.AccountApi;
using IAccountApi = Sybon.Auth.Client.Api.IAccountApi;
using IPermissionsApi = Sybon.Auth.Client.Api.IPermissionsApi;
using PermissionsApi = Sybon.Auth.Client.Api.PermissionsApi;
using Submit = Sybon.Checking.Services.SubmitService.Models.Submit;

namespace Sybon.Checking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SecurityConfiguration = new CheckingSecurityConfiguration(configuration.GetSection("Security"));
        }

        private IConfiguration Configuration { get; }
        private CheckingSecurityConfiguration SecurityConfiguration { get; }

        private Auth.Client.Client.Configuration AuthClientConfiguration => new Auth.Client.Client.Configuration
        {
            BasePath = SecurityConfiguration.SybonAuth.Url,
            ApiKey = new Dictionary<string, string>
            {
                {"api_key", SecurityConfiguration.ApiKey}
            }
        };
        
        private Archive.Client.Client.Configuration ArchiveClientConfiguration => new Archive.Client.Client.Configuration
        {
            BasePath = SecurityConfiguration.SybonArchive.Url,
            ApiKey = new Dictionary<string, string>
            {
                {"api_key", SecurityConfiguration.ApiKey}
            }
        };

        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Sybon.Checking", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.AddSecurityDefinition("api_key", new ApiKeyScheme {In = "query", Name = "api_key"});
                c.OperationFilter<SwaggerApiKeySecurityFilter>();
            });
            
            services.AddDbContext<CheckingContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddScoped<IRepositoryUnitOfWork, RepositoryUnitOfWork<CheckingContext>>();
            
            services.AddScoped<ICompilersRepository, CompilersRepository>();
            services.AddScoped<ICompilersService, CompilersService>();
            
            services.AddScoped<ISubmitsRepository, SubmitsRepository>();
            services.AddScoped<ISubmitService, SubmitService>();
            
            services.AddScoped<ISubmitResultRepository, SubmitResultRepository>();
            services.AddScoped<ISubmitResultService, SubmitResultService>();
            
            services.AddSingleton<IAccountApi>(serviceProvider => new AccountApi(AuthClientConfiguration));
            services.AddSingleton<IPermissionsApi>(serviceProvider => new PermissionsApi(AuthClientConfiguration));
            services.AddSingleton<IProblemsApi>(serviceProvider => new ProblemsApi(ArchiveClientConfiguration));
            services.AddSingleton<IArchiveClient, IArchiveClient>(CreateArchiveClient);
            services.AddSingleton<SubmitClient, SubmitClient>(CreateSubmitClient);
            services.AddSingleton<IMapper, IMapper>(CreateMapper);
            services.AddSingleton<ISubmitCallbackService, SubmitCallbackService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sybon.Checking V1");
            });
            app.UseMvc();

            if(SecurityConfiguration.BunsanBroker.Enabled)
                app.ApplicationServices
                    .GetService<ISubmitCallbackService>()
                    .Listen(GetBunsanConnectionParameters(SecurityConfiguration.BunsanBroker));
        }
        
        private static IMapper CreateMapper(IServiceProvider serviceProvider)
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Compiler, Services.CompilersService.Models.Compiler>()
                    .ForMember(x => x.MemoryLimitBytes, opt => opt.MapFrom(src => src.ResourceLimits.MemoryLimitBytes))
                    .ForMember(x => x.NumberOfProcesses, opt => opt.MapFrom(src => src.ResourceLimits.NumberOfProcesses))
                    .ForMember(x => x.OutputLimitBytes, opt => opt.MapFrom(src => src.ResourceLimits.OutputLimitBytes))
                    .ForMember(x => x.TimeLimitMillis, opt => opt.MapFrom(src => src.ResourceLimits.TimeLimitMillis))
                    .ForMember(x => x.RealTimeLimitMillis, opt => opt.MapFrom(src => src.ResourceLimits.RealTimeLimitMillis));

                config.CreateMap<Submit, Repositories.SubmitsRepository.Submit>()
                    .ForMember(x => x.Solution, opt => opt.MapFrom(src => new Solution
                    {
                        FileType = Mapper.Instance.Map<Solution.SolutionFileType>(src.SolutionFileType),
                        Data = src.Solution
                    }));

                config.CreateMap<Repositories.SubmitsRepository.Submit, Submit>()
                    .ForMember(x => x.Solution, opt => opt.MapFrom(src => src.Solution.Data))
                    .ForMember(x => x.SolutionFileType, opt => opt.MapFrom(src => src.Solution.FileType));
                config.CreateMap<SubmitResult, Services.SubmitResultService.Models.SubmitResult>();
                config.CreateMap<BuildResult, Services.SubmitResultService.Models.BuildResult>();
                config.CreateMap<TestGroupResult, Services.SubmitResultService.Models.TestGroupResult>();
                config.CreateMap<TestResult, Services.SubmitResultService.Models.TestResult>();
                config.CreateMap<ResourceUsage, Services.SubmitResultService.Models.ResourceUsage>();
                config.CreateMap<SubmitsController.SubmitModel, Submit>();
            });
            return Mapper.Instance;
        }
        
        private IArchiveClient CreateArchiveClient(IServiceProvider serviceProvider)
        {
            var config = SecurityConfiguration.BacsArchive;
            return ArchiveClientFactory.CreateFromFiles(
                config.Host,
                config.Port,
                config.ClientCertificatePath,
                config.ClientKeyPath,
                config.CAPath
            );
        }
        
        private SubmitClient CreateSubmitClient(IServiceProvider serviceProvider)
        {
            var config = SecurityConfiguration.BunsanBroker;
            var configuration = new Configuration
            {
                WorkerName = config.WorkerName,
                WorkerResourceName = config.WorkerResourceName,
                ConnectionParameters = GetBunsanConnectionParameters(config)
            };
            return new SubmitClient(configuration);
        }

        private static ConnectionParameters GetBunsanConnectionParameters(BunsanBrokerConfiguration config)
        {
            return new ConnectionParameters
            {
                Host = config.Host,
                Port = config.Port,
                VirtualHost = config.VirtualHost,
                Identifier = config.Identifier,
                Credentials = new Credentials
                {
                    Username = config.Username,
                    Password = config.Password
                }
            };
        }
    }
}