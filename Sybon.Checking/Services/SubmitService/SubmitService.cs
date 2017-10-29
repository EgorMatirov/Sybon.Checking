using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Bacs.Archive.Client.CSharp;
using Bacs.Process;
using Bacs.Utility;
using Bunsan.Broker;
using Google.Protobuf;
using JetBrains.Annotations;
using Sybon.Archive.Client.Api;
using Sybon.Checking.Repositories.CompilersRepository;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Checking.Repositories.SubmitsRepository;
using Sybon.Common;
using Submit = Sybon.Checking.Services.SubmitService.Models.Submit;
using Task = System.Threading.Tasks.Task;

namespace Sybon.Checking.Services.SubmitService
{
    [UsedImplicitly]
    public class SubmitService : ISubmitService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IMapper _mapper;
        private readonly IProblemsApi _problemsApi;
        private readonly IArchiveClient _archiveClient;
        private readonly SubmitClient _submitClient;

        public SubmitService(IRepositoryUnitOfWork repositoryUnitOfWork, IMapper mapper, IProblemsApi problemsApi, IArchiveClient archiveClient, SubmitClient submitClient)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
            _problemsApi = problemsApi;
            _archiveClient = archiveClient;
            _submitClient = submitClient;
        }

        public async Task<long> SendAsync(Submit submit)
        {
            var dbEntry = _mapper.Map<Repositories.SubmitsRepository.Submit>(submit);
            /*
             * TODO: Don't save changes until data is sent by submitClient.
             * Probably we need to use GUIDs as identifiers instead of ids so we are able to generate guid and use it as
             * identifier without saving data. Or just introduce a separated indetifier for submit result (guid) and save it somewhere.
            */
            dbEntry.Result = new SubmitResult();
            await _repositoryUnitOfWork.GetRepository<ISubmitsRepository>().AddAsync(dbEntry);
            await _repositoryUnitOfWork.SaveChangesAsync();
            return await SendAsync(dbEntry);
        }

        private async Task<long> SendAsync(Repositories.SubmitsRepository.Submit dbEntry)
        {
            var problem = await _problemsApi.GetByIdAsync(dbEntry.ProblemId);

            var problemInfo = _archiveClient
                .ImportResult(problem.InternalProblemId)
                ?.GetValueOrDefault(problem.InternalProblemId)
                ?.Problem;

            var compiler = await _repositoryUnitOfWork.GetRepository<ICompilersRepository>().FindAsync(dbEntry.CompilerId);
            // TODO: Check that compiler is obtained correctly.
            var resourceLimits = compiler.ResourceLimits;
            var solution = new Buildable
            {
                Source = new Source
                {
                    Data = ByteString.CopyFrom(dbEntry.Solution.Data),
                    Archiver = GetArchiver(dbEntry.Solution.FileType)
                },
                BuildSettings = new BuildSettings
                {
                    ResourceLimits = new Bacs.Process.ResourceLimits
                    {
                        MemoryLimitBytes = (ulong) resourceLimits.MemoryLimitBytes,
                        TimeLimitMillis = (ulong) resourceLimits.TimeLimitMillis,
                        NumberOfProcesses = (ulong) resourceLimits.NumberOfProcesses,
                        OutputLimitBytes = (ulong) resourceLimits.OutputLimitBytes,
                        RealTimeLimitMillis = (ulong) resourceLimits.RealTimeLimitMillis
                    },
                    Config = new BuilderConfig
                    {
                        Type = compiler.Type,
                        Argument = {compiler.ArgList}
                    }
                }
            };

            var solutionData = new SubmitData
            {
                Identifier = dbEntry.ResultId.ToString(),
                Problem = problemInfo,
                Solution = solution,
                PretestsOnly = dbEntry.PretestsOnly
            };
            _submitClient.Submit(solutionData);
            return dbEntry.Id;
        }

        public async Task<Submit> GetAsync(long id)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<ISubmitsRepository>().FindAsync(id);
            return _mapper.Map<Submit>(dbEntry);
        }

        public async Task RejudgeAsync(long id)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<ISubmitsRepository>().FindAsync(id);
            await SendAsync(dbEntry);
        }

        private static Archiver GetArchiver(Solution.SolutionFileType solutionFileType)
        {
            switch (solutionFileType)
            {
                case Solution.SolutionFileType.Text:
                    return null;
                case Solution.SolutionFileType.Zip:
                    return new Archiver
                    {
                        Type = "7z",
                        Format = "zip"
                    };
                default:
                    throw new InvalidOperationException($"Unknown SolutionFileType: {solutionFileType}");
            }
        }
    }
}