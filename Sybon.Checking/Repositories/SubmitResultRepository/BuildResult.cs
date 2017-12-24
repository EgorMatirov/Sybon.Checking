using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    public class BuildResult : IEntity
    {
        public enum BuildStatus {
            OK = 0,
            FAILED = 1,
            PENDING = 1000,
            SERVER_ERROR = 1001
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public BuildStatus Status { get; set; }
        public byte[] Output { get; set; }
    }
}