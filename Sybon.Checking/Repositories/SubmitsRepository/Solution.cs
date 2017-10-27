using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitsRepository
{
    public class Solution : IEntity
    {
        public enum SolutionFileType
        {
            Text,
            Zip
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public byte[] Data { get; set; }
        public SolutionFileType FileType { get; set; }
        public Submit Submit { get; set; }
    }
}