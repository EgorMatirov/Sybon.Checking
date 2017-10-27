using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    public class TestResult : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long TestGroupResultId { get; set; }
        public TestGroupResult TestGroupResult { get; set; }
        public TestResultStatus Status { get; set; }
        public string JudgeMessage { get; set; }
        public long? ResourceUsageId { get; set; }
        public ResourceUsage ResourceUsage { get; set; }
        public string Input { get; set; }
        public string ActualResult { get; set; }
        public string ExpectedResult { get; set; }
    }
}