using System.Threading.Tasks;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitsRepository
{
    public interface ISubmitsRepository : IBaseEntityRepository<Submit>
    {
        Task<Submit[]> GetAllAsync(long[] ids, bool fetchFields);
    }
}