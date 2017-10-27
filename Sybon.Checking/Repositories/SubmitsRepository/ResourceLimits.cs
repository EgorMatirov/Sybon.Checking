using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitsRepository
{
    public class ResourceLimits : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long TimeLimitMillis { get; set; }
        public long MemoryLimitBytes { get; set; }
        public long NumberOfProcesses { get; set; }
        public long OutputLimitBytes { get; set; }
        public long RealTimeLimitMillis { get; set; }
    }
}