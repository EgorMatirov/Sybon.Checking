using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Sybon.Checking.Repositories.CompilersRepository;
using Sybon.Common;
using Compiler = Sybon.Checking.Services.CompilersService.Models.Compiler;

namespace Sybon.Checking.Services.CompilersService
{
    [UsedImplicitly]
    public class CompilersService : ICompilersService
    {
        private readonly IRepositoryUnitOfWork _repositoryUnitOfWork;
        private readonly IMapper _mapper;

        public CompilersService(IRepositoryUnitOfWork repositoryUnitOfWork, IMapper mapper)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _mapper = mapper;
        }

        public async Task<Compiler[]> GetAllAsync()
        {
            var compilers = await _repositoryUnitOfWork.GetRepository<ICompilersRepository>().GetAllAsync();
            return _mapper.Map<Compiler[]>(compilers);
        }
    }
}