using System.Linq;
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

        public Task<Submit[]> GetAllAsync(long[] ids, bool fetchFields)
        {
            var submits = Context.Submits.Where(x => ids.Contains(x.Id));
            if (fetchFields)
                submits = submits
                    .Include(x => x.Result)
                    .ThenInclude(x => x.BuildResult)
                    .Include(x => x.Result)
                    .ThenInclude(x => x.TestGroupResults)
                    .ThenInclude(x => x.TestResults)
                    .ThenInclude(x => x.ResourceUsage)
                    .Include(x => x.Solution);
             return submits.ToArrayAsync();
        }
    }
}