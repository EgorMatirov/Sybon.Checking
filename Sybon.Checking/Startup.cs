﻿using System.Collections.Generic;
using Bunsan.Broker;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sybon.Archive.Client.Api;
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

        private Archive.Client.Client.Configuration ArchiveClientConfiguration =>
            new Archive.Client.Client.Configuration
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

            services.AddSwagger("Sybon.Checking", "v1");

            services.AddDbContext<CheckingContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services
                .AddScoped<IRepositoryUnitOfWork, RepositoryUnitOfWork<CheckingContext>>()
                .AddScoped<ICompilersRepository, CompilersRepository>()
                .AddScoped<ICompilersService, CompilersService>()
                .AddScoped<ISubmitsRepository, SubmitsRepository>()
                .AddScoped<ISubmitService, SubmitService>()
                .AddScoped<ISubmitResultRepository, SubmitResultRepository>()
                .AddScoped<ISubmitResultService, SubmitResultService>()
                .AddSingleton<IAccountApi>(serviceProvider => new AccountApi(AuthClientConfiguration))
                .AddSingleton<IPermissionsApi>(serviceProvider => new PermissionsApi(AuthClientConfiguration))
                .AddSingleton<IProblemsApi>(serviceProvider => new ProblemsApi(ArchiveClientConfiguration))
                .AddArchiveClient(SecurityConfiguration.BacsArchive)
                .AddSubmitClient(SecurityConfiguration.BunsanBroker)
                .AddSingleton<ISubmitCallbackService, SubmitCallbackService>();

            services.AddMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sybon.Checking V1"); });
            app.UseMvc();

            if (SecurityConfiguration.BunsanBroker.Enabled)
                app.ApplicationServices
                    .GetService<ISubmitCallbackService>()
                    .Listen(GetBunsanConnectionParameters(SecurityConfiguration.BunsanBroker));
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