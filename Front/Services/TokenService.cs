using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace Front.Services
{
    public class TokenService
    {
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";

        public TokenService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetTokenAsync(string token)
        {
            await _localStorage.SetItemAsync(TokenKey, token);
        }

        public async Task<string> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        public async Task RemoveTokenAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
        }
    }
}
