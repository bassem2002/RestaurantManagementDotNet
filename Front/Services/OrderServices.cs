using System.Net.Http.Json;
using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services
{
    public class AdminOrderService
    {
        private readonly HttpClient _http;

        public AdminOrderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var result = await _http.GetFromJsonAsync<List<OrderDto>>("/api/order");
            return result ?? new List<OrderDto>();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            return await _http.GetFromJsonAsync<OrderDto>($"/api/order/{orderId}");
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var payload = new { status };
            await _http.PutAsJsonAsync($"/api/order/{orderId}/status", payload);
        }
    }

    // ==================== SERVICE CLIENT ====================
    public class OrderClientService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public OrderClientService(HttpClient http, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _authStateProvider = authStateProvider;
        }

        /// <summary>
        /// Récupère les commandes de l'utilisateur connecté
        /// </summary>
        public async Task<List<OrderDTO>> GetUserOrdersAsync()
        {
            try
            {
                // Récupérer l'état d'authentification et l'ID utilisateur
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var userId = authState.User.FindFirst("sub")?.Value 
                    ?? authState.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return new List<OrderDTO>();

                return await _http.GetFromJsonAsync<List<OrderDTO>>($"/api/order/user/{userId}") ?? new List<OrderDTO>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur récupération commandes: {ex.Message}");
                return new List<OrderDTO>();
            }
        }

        /// <summary>
        /// Récupère une commande spécifique par ID
        /// </summary>
        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            try
            {
                Console.WriteLine($"[OrderClientService] Récupération de la commande {orderId}");
                var result = await _http.GetFromJsonAsync<OrderDTO>($"/api/order/{orderId}");
                
                if (result == null)
                {
                    Console.Error.WriteLine($"[OrderClientService] Commande {orderId} est null");
                    return null;
                }

                Console.WriteLine($"[OrderClientService] Commande {orderId} chargée avec succès: OrderId={result.OrderId}, Items={result.OrderItems?.Count ?? 0}");
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[OrderClientService] Erreur récupération commande {orderId}: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                    Console.Error.WriteLine($"[OrderClientService] InnerException: {ex.InnerException.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crée une nouvelle commande
        /// </summary>
        public async Task<OrderDTO> CreateOrderAsync(CheckoutRequest checkoutRequest)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/api/order", checkoutRequest);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<OrderDTO>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur création commande: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Récupère le statut d'une commande
        /// </summary>
        public async Task<string> GetOrderStatusAsync(int orderId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/order/{orderId}/status");
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur statut commande: {ex.Message}");
                return "Erreur";
            }
        }

        /// <summary>
        /// Annule une commande
        /// </summary>
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"/api/order/{orderId}/cancel", new { });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur annulation commande: {ex.Message}");
                return false;
            }
        }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }

        public PaymentDto? Payment { get; set; }
        public ClientDto? Client { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public string? Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; } = "Unpaid";
    }

    public class ClientDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }

    public class OrderItemDto
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
