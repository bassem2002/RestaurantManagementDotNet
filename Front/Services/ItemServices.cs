using Backend.Models;
using Front.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Front.Services
{
    public class ItemServices
    {
        private readonly HttpClient httpClient;

        public ItemServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        // ---------------------- Token ----------------------
        public void SetToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // ---------------------- GET (public) ----------------------
        public async Task<List<Item>> GetItemsAsync()
        {
            return await httpClient.GetFromJsonAsync<List<Item>>("api/item") ?? new List<Item>();
        }

        public async Task<Item> GetItemAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<Item>($"api/item/{id}");
        }

        // ---------------------- POST (admin) ----------------------
        public async Task<Item> CreateItemAsync(Item item)
        {
            var result = await httpClient.PostAsJsonAsync("api/item", item);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<Item>();
        }

        // ---------------------- PUT (admin) ----------------------
        public async Task<bool> UpdateItemAsync(Item item)
        {
            var result = await httpClient.PutAsJsonAsync($"api/item/{item.ItemId}", item);
            return result.IsSuccessStatusCode;
        }

        // ---------------------- DELETE (admin) ----------------------
        public async Task<bool> DeleteItemAsync(int id)
        {
            var result = await httpClient.DeleteAsync($"api/item/{id}");
            return result.IsSuccessStatusCode;
        }
    }
}
