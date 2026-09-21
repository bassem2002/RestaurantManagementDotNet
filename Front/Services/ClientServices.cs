using Backend.Models;
using System.Net.Http.Json;

public class ClientServices
{
    private readonly HttpClient _http;
    public ClientServices(HttpClient http) => _http = http;

    public async Task<List<ApplicationUser>> GetClients() =>
        await _http.GetFromJsonAsync<List<ApplicationUser>>("api/Clients");

    public async Task<List<CartItem>> GetCartByClientId(string clientId) =>
        await _http.GetFromJsonAsync<List<CartItem>>($"api/Carts/{clientId}");

    public async Task<List<CartItem>> GetCartItemsByCartId(int cartId)
    {
        return await _http.GetFromJsonAsync<List<CartItem>>($"api/Carts/{cartId}");
    }
}

