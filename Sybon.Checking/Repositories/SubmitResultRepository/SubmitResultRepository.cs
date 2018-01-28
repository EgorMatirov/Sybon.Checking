using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    [UsedImplicitly]
    public class SubmitResultRepository : BaseEntityRepository<SubmitResult, CheckingContext>, ISubmitResultRepository
    {
        public SubmitResultRepository(CheckingContext context) : base(context)
        {
        }

        public new Task<SubmitResult> FindAsync(long key)
        {
            return Context.SubmitResult
                .Include(x => x.BuildResult)
                .Include(x => x.TestGroupResults)
                .FirstOrDefaultAsync(x => x.Id == key);
        }

        public Task<SubmitResult[]> GetAllBySubmitIdsAsync(long[] submitIds)
        {
            return Context.SubmitResult
                .Where(x => submitIds.Contains(x.Submit.Id))
                .Include(x => x.BuildResult)
                .Include(x => x.TestGroupResults)
                .ThenInclude(x => x.TestResults)
                .ThenInclude(x => x.ResourceUsage)
                .ToArrayAsync();
        }
    }
}