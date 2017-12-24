namespace Sybon.Checking.Services.SubmitResultService.Models
{
    public class BuildResult
    {
        public enum BuildStatus {
            OK = 0,
            FAILED = 1,
            PENDING = 1000
        }
        public BuildStatus Status { get; set; }
        public byte[] Output { get; set; }
    }
}