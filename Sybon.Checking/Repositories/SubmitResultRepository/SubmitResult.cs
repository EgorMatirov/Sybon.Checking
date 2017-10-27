using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Checking.Repositories.SubmitsRepository;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitResultRepository
{
    public class SubmitResult : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long? BuildResultId { get; set; }
        public BuildResult BuildResult { get; set; }
        public Submit Submit { get; set; }
        public ICollection<TestGroupResult> TestResults { get; set; }
    }
}