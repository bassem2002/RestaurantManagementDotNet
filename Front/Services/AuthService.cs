using System.Net.Http.Json;
using Front.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Front.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly TokenService _tokenService;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public AuthService(HttpClient http, TokenService tokenService, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _tokenService = tokenService;
            _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        }

        // ============================ LOGIN ============================
        public async Task<(bool IsSuccess, string Error)> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Account/login", request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, string.IsNullOrWhiteSpace(error) ? "Nom d'utilisateur ou mot de passe incorrect." : error);
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.token))
                    return (false, "Réponse invalide du serveur (token manquant).");

                // Save token
                await _tokenService.SetTokenAsync(loginResponse.token);

                // Notify auth state provider
                _authStateProvider.NotifyUserAuthentication(loginResponse.token);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // ============================ REGISTER ============================
        public async Task<(bool IsSuccess, string Error)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Account/register", request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return (false, string.IsNullOrWhiteSpace(error) ? "Erreur lors de l'inscription." : error);
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // ============================ LOGOUT ============================
        public async Task LogoutAsync()
        {
            // Supprime le token
            await _tokenService.RemoveTokenAsync();

            // Met à jour l’état d’authentification
            _authStateProvider.NotifyUserLogout();
        }

        // ============================ GET TOKEN ============================
        public async Task<string> GetTokenAsync()
        {
            return await _tokenService.GetTokenAsync();
        }
    }

    // ============================ LOGIN RESPONSE ============================
    public class LoginResponse
    {
        public string token { get; set; }
        public string[] roles { get; set; }
        public string username { get; set; }
        public string userId { get; set; }
    }
}
