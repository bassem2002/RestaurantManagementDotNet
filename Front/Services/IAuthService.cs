using System.Threading.Tasks;
using Front.Models;

namespace Front.Services
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string Error)> LoginAsync(LoginRequest request);
        Task<(bool IsSuccess, string Error)> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
        Task<string> GetTokenAsync();
    }
}
