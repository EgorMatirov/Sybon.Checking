using System.Threading.Tasks;
using Sybon.Checking.Services.SubmitResultService.Models;

namespace Sybon.Checking.Services.SubmitResultService
{
    public interface ISubmitResultService
    {
        Task<SubmitResult[]> GetAllBySubmitIdsAsync(long[] submitIds);
    }
}