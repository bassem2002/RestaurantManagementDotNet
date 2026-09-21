using Backend.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Front.Services
{
    public class CategorieServices
    {
        private readonly HttpClient _http;
        private readonly TokenService _tokenService;

        public CategorieServices(HttpClient http, TokenService tokenService)
        {
            _http = http;
            _tokenService = tokenService;
        }

        // ---------------------- Private: Set Authorization Header ----------------------
        private async Task SetAuthHeaderAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            _http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }

        // ---------------------- GET (public) ----------------------
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Category>>("api/Category")
                       ?? new List<Category>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"GetCategoriesAsync Error: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Category>($"api/Category/{id}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"GetCategoryByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        // ---------------------- POST (admin) ----------------------
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await SetAuthHeaderAsync();

            var response = await _http.PostAsJsonAsync("api/Category", category);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Category>();
        }

        // ---------------------- PUT (admin) ----------------------
        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            await SetAuthHeaderAsync();

            var response = await _http.PutAsJsonAsync($"api/Category/{id}", category);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Category>();
        }

        // ---------------------- DELETE (admin) ----------------------
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            await SetAuthHeaderAsync();

            var response = await _http.DeleteAsync($"api/Category/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
