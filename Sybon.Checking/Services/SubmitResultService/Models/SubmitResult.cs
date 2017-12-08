using System.Collections.Generic;

namespace Sybon.Checking.Services.SubmitResultService.Models
{
    public class SubmitResult
    {
        public long Id { get; set; }
        public BuildResult BuildResult { get; set; }
        public ICollection<TestGroupResult> TestGroupResults { get; set; }
    }
}