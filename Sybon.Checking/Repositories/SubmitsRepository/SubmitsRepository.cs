using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitsRepository
{
    [UsedImplicitly]
    public class SubmitsRepository : BaseEntityRepository<Submit, CheckingContext>, ISubmitsRepository
    {
        public SubmitsRepository(CheckingContext context) : base(context)
        {
        }

        public new Task<Submit> FindAsync(long key)
        {
            return Context.Submits
                .Include(x => x.Result)
                .ThenInclude(x => x.BuildResult)
                .Include(x => x.Result)
                .ThenInclude(x => x.TestResults)
                .ThenInclude(x => x.TestResults)
                .ThenInclude(x => x.ResourceUsage)
                .FirstOrDefaultAsync(x => x.Id == key);
        }
    }
}