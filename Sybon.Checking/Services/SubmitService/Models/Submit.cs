using System;
using Sybon.Checking.Services.SubmitResultService.Models;

namespace Sybon.Checking.Services.SubmitService.Models
{
    public class Submit
    {
        public long Id { get; set; }
        public long CompilerId { get; set; }
        public byte[] Solution { get; set; }
        public SolutionFileType SolutionFileType { get; set; }
        public long ProblemId { get; set; }
        public DateTime Created { get; set; }
        public long? UserId { get; set; }
        public SubmitResult Result { get; set; }
        public bool PretestsOnly { get; set; }
    }
}