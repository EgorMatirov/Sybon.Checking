using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Common;

namespace Sybon.Checking.Repositories.CompilersRepository
{
    [UsedImplicitly]
    public class CompilersRepository : BaseEntityRepository<Compiler, CheckingContext>, ICompilersRepository
    {
        public CompilersRepository(CheckingContext context) : base(context)
        {
        }

        public Task<Compiler[]> GetAllAsync()
        {
            return Context.Compilers
                .Include(c => c.ResourceLimits)
                .ToArrayAsync();
        }
        
        public new Task<Compiler> FindAsync(long id)
        {
            return Context.Compilers
                .Include(c => c.ResourceLimits)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}