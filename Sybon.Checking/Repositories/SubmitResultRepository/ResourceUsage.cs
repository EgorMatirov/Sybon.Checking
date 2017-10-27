using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    public class ResourceUsage : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long TimeUsageMillis { get; set; }
        public long MemoryUsageBytes { get; set; }
    }
}