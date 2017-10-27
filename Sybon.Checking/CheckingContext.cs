using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sybon.Checking.Repositories.CompilersRepository;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Checking.Repositories.SubmitsRepository;

namespace Sybon.Checking
{
    public class CheckingContext : DbContext
    {
        public CheckingContext(DbContextOptions<CheckingContext> options) : base(options)
        {
        }

        public DbSet<Compiler> Compilers { get; [UsedImplicitly] set; }
        public DbSet<Submit> Submits { get; [UsedImplicitly] set; }
        public DbSet<SubmitResult> SubmitResult { get; [UsedImplicitly] set; }
    }
}