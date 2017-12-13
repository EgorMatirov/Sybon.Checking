namespace Sybon.Checking.Services.CompilersService.Models
{
    public class Compiler
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public long TimeLimitMillis { get; set; }
        public long MemoryLimitBytes { get; set; }
        public long NumberOfProcesses { get; set; }
        public long OutputLimitBytes { get; set; }
        public long RealTimeLimitMillis { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Args { get; set; }
    }
}