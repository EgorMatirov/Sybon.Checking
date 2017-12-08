using System;
using System.ComponentModel.DataAnnotations.Schema;
using Sybon.Checking.Repositories.CompilersRepository;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Common;

namespace Sybon.Checking.Repositories.SubmitsRepository
{
    public class Submit : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long SolutionId { get; set; }
        public Solution Solution { get; set; }
        public long CompilerId { get; set; }
        public Compiler Compiler { get; set; }
        public long? ResultId { get; set; }
        public SubmitResult Result { get; set; }
        public long ProblemId { get; set; }
        public DateTime Created { get; set; }
        public long? UserId { get; set; }
        public bool PretestsOnly { get; set; }
        public ContinueCondition ContinueCondition { get; set; }
    }
}