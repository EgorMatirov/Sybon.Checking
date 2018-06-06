using System.Linq;
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

        public async Task<SubmitResult[]> GetAllBySubmitIdsAsync(long[] submitIds)
        {
            var dbEntry = await _repositoryUnitOfWork.GetRepository<ISubmitResultRepository>()
                .GetAllBySubmitIdsAsync(submitIds);
            foreach (var submitResult in dbEntry)
            {
                foreach (var testGroupResult in submitResult.TestGroupResults)
                {
                    testGroupResult.TestResults = testGroupResult.TestResults.OrderBy(x => x.OrderNumber).ToList();
                }

                submitResult.TestGroupResults = submitResult.TestGroupResults.OrderBy(x => x.OrderNumber).ToList();
            }

            return _mapper.Map<SubmitResult[]>(dbEntry);
        }
    }
}