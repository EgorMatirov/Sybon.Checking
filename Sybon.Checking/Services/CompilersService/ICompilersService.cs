using System.Threading.Tasks;
using Sybon.Checking.Services.CompilersService.Models;

namespace Sybon.Checking.Services.CompilersService
{
    public interface ICompilersService
    {
        Task<Compiler[]> GetAllAsync();
    }
}