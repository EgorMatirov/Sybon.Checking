using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Checking.Repositories.SubmitsRepository;

namespace Sybon.Checking.Services.SubmitResultService.Models
{
    public class TestResult
    {
        public TestResultStatus Status { get; set; }
        public string JudgeMessage { get; set; }
        public ResourceUsage ResourceUsage { get; set; }
        public string Input { get; set; }
        public string ActualResult { get; set; }
        public string ExpectedResult { get; set; }
    }
}