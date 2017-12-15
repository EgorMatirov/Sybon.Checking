using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bunsan.Broker;
using Microsoft.Extensions.DependencyInjection;
using Sybon.Checking.Controllers;
using Sybon.Checking.Repositories.CompilersRepository;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Checking.Repositories.SubmitsRepository;
using Submit = Sybon.Checking.Services.SubmitService.Models.Submit;

namespace Sybon.Checking
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddArchiveClient(this IServiceCollection services,
            CheckingSecurityConfiguration.BacsArchiveConfiguration config)
        {
            return services.AddSingleton<IArchiveClient, IArchiveClient>(provider =>
                ArchiveClientFactory.CreateFromFiles(
                    config.Host,
                    config.Port,
                    config.ClientCertificatePath,
                    config.ClientKeyPath,
                    config.CAPath
                ));
        }

        public static IServiceCollection AddSubmitClient(this IServiceCollection services,
            BunsanBrokerConfiguration config)
        {
            return services.AddSingleton<SubmitClient, SubmitClient>(provider =>
                new SubmitClient(new Configuration
                {
                    WorkerName = config.WorkerName,
                    WorkerResourceName = config.WorkerResourceName,
                    ConnectionParameters = GetBunsanConnectionParameters(config)
                }));
        }

        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Compiler, Services.CompilersService.Models.Compiler>()
                    .ForMember(x => x.MemoryLimitBytes, opt => opt.MapFrom(src => src.ResourceLimits.MemoryLimitBytes))
                    .ForMember(x => x.NumberOfProcesses,
                        opt => opt.MapFrom(src => src.ResourceLimits.NumberOfProcesses))
                    .ForMember(x => x.OutputLimitBytes, opt => opt.MapFrom(src => src.ResourceLimits.OutputLimitBytes))
                    .ForMember(x => x.TimeLimitMillis, opt => opt.MapFrom(src => src.ResourceLimits.TimeLimitMillis))
                    .ForMember(x => x.RealTimeLimitMillis,
                        opt => opt.MapFrom(src => src.ResourceLimits.RealTimeLimitMillis));

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
            return services.AddSingleton<IMapper, IMapper>(s => Mapper.Instance);
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