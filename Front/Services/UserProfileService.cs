using Front.Models;
using System.Net.Http.Json;

namespace Front.Services
{
    public class UserProfileService
    {
        private readonly HttpClient _http;

        public UserProfileService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Récupère le profil de l'utilisateur actuellement connecté
        /// </summary>
        public async Task<UserProfileDTO> GetUserProfileAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Clients/profile");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erreur {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<UserProfileDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur UserProfileService: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Met à jour le profil de l'utilisateur
        /// </summary>
        public async Task<bool> UpdateUserProfileAsync(UserProfileDTO profile)
        {
            try
            {
                var response = await _http.PutAsJsonAsync("api/Clients/profile", profile);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur mise à jour profil: {ex.Message}");
                return false;
            }
        }
    }
}
