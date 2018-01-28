using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    public interface ISubmitResultRepository : IBaseEntityRepository<SubmitResult>
    {
        Task<SubmitResult[]> GetAllBySubmitIdsAsync(long[] submitIds);
    }
}