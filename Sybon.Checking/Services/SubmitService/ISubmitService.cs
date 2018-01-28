using System.Threading.Tasks;
using Sybon.Checking.Services.SubmitService.Models;

namespace Sybon.Checking.Services.SubmitService
{
    public interface ISubmitService
    {
        Task<long> SendAsync(Submit submit);
        Task RejudgeAsync(long id);
        Task<Submit[]> GetAllAsync(long[] ids, bool fetchFields = true);
    }
}