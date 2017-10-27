using System.Collections.Generic;

namespace Sybon.Checking.Services.SubmitResultService.Models
{
    public class TestGroupResult
    {
        public string InternalId { get; set; }
        public bool Executed { get; set; }
        public ICollection<TestResult> TestResults { get; set; }
    }
}