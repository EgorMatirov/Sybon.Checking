using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Sybon.Common;

namespace Sybon.Checking.Repositories.CompilersRepository
{
    public class Compiler : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Type { get; set; }
        public long ResourceLimitsId { get; set; }
        public ResourceLimits ResourceLimits { get; set; }
        public string Args { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> ArgList => Args.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }
}