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
            return _mapper.Map<SubmitResult[]>(dbEntry);
        }
    }
}