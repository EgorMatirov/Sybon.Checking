using System.Threading.Tasks;
using AutoMapper;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Common;
using SubmitResult = Sybon.Checking.Services.SubmitResultService.Models.SubmitResult;

namespace Sybon.Checking.Services.SubmitResultService
{
    public class SubmitResultService : ISubmitResultService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IMapper _mapper;

        public SubmitResultService(IRepositoryUnitOfWork repositoryUnitOfWork, IMapper mapper)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
        }

        public async Task<SubmitResult> GetAsync(long id)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<ISubmitResultRepository>().FindAsync(id);
            return _mapper.Map<SubmitResult>(dbEntry);
        }
//
//        public override long Add(SubmitResult entry)
//        {
//            var dbEntry = MapperUoW.SubmitResultMapper.InverseMap(entry);
//            AddProps(dbEntry);
//            dbEntry = RepositoryUoW.SubmitResultRepository.Add(dbEntry);
//            RepositoryUoW.Save();
//            return dbEntry.Id;
//        }
//
//        public override void Update(SubmitResult entry)
//        {
//            var dbEntry = MapperUoW.SubmitResultMapper.InverseMap(entry);
//            AddProps(dbEntry);
//            dbEntry = RepositoryUoW.SubmitResultRepository.Update(dbEntry);
//            RepositoryUoW.Save();
//        }
//
//        public override void Remove(long id)
//        {
//            RepositoryUoW.SubmitResultRepository.Remove(id);
//        }
//
//        protected virtual void AddProps(Db.SubmitResult dbEntry)
//        {
//            var buildResultRepo = RepositoryUoW.GetRepo<Db.BuildResult>();
//            var testGroupResultRepo = RepositoryUoW.GetRepo<Db.TestGroupResult>();
//            var resourceUsageRepo = RepositoryUoW.GetRepo<Db.ResourceUsage>();
//
//            dbEntry.BuildResult = buildResultRepo.Add(dbEntry.BuildResult);
//            foreach (var testGroupResult in dbEntry.TestResults)
//            {
//                foreach (var testResult in testGroupResult.TestResults.Where(x => x.ResourceUsage != null))
//                {
//                    testResult.ResourceUsage = resourceUsageRepo.Add(testResult.ResourceUsage);
//                    RepositoryUoW.Save();
//                }
//                testGroupResult.SubmitResultId = dbEntry.Id;
//            }
//            dbEntry.TestResults = testGroupResultRepo.AddRange(dbEntry.TestResults);
//            RepositoryUoW.Save();
//        }
    }
}