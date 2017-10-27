using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    public class TestGroupResult : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string InternalId { get; set; }
        public long SubmitResultId { get; set; }
        public SubmitResult SubmitResult { get; set; }
        public bool Executed { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
    }
}