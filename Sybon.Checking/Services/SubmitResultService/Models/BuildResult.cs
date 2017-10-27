namespace Sybon.Checking.Services.SubmitResultService.Models
{
    public class BuildResult
    {
        public Bacs.Process.BuildResult.Types.Status Status { get; set; }
        public byte[] Output { get; set; }
    }
}