using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Checking.Repositories.CompilersRepository
{
    public interface ICompilersRepository : IBaseEntityRepository<Compiler>
    {
        Task<Compiler[]> GetAllAsync();
    }
}