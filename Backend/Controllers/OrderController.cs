using Backend.DTOs;
using Backend.Models;
using Backend.Repository.Implementations;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderController(IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderRepository.GetOrders();

            var ordersDto = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                UserId = o.UserId,
                Username = o.User?.UserName,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                City = o.City,
                PostalCode = o.PostalCode,
                Phone = o.Phone,
                Notes = o.Notes,
                OrderItems = o.OrderItems?.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderId = oi.OrderId,
                    ItemId = oi.ItemId,
                    ItemName = oi.Item?.Name,
                    Item = oi.Item != null ? new ItemDTO
                    {
                        ItemId = oi.Item.ItemId,
                        Name = oi.Item.Name,
                        Price = oi.Item.Price,
                        CategoryName = oi.Item.Category?.Name
                    } : null
                }).ToList() ?? new List<OrderItemDTO>()
            }).ToList();

            return Ok(ordersDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            try
            {
                Console.WriteLine($"[OrderController] GetOrderById({id}) appelé");
                var order = await _orderRepository.GetOrderById(id);

                if (order == null)
                {
                    Console.WriteLine($"[OrderController] Commande {id} non trouvée");
                    return NotFound();
                }

                Console.WriteLine($"[OrderController] Commande trouvée: Status={order.Status}, OrderItems={order.OrderItems?.Count ?? 0}");

                var dto = new OrderDTO
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    UserId = order.UserId,
                    Username = order.User?.UserName,
                    Status = order.Status,
                    ShippingAddress = order.ShippingAddress,
                    City = order.City,
                    PostalCode = order.PostalCode,
                    Phone = order.Phone,
                    Notes = order.Notes,
                    OrderItems = order.OrderItems?.Select(oi => new OrderItemDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ItemId = oi.ItemId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        ItemName = oi.Item?.Name ?? "Article supprimé",
                        Item = oi.Item != null ? new ItemDTO
                        {
                            ItemId = oi.Item.ItemId,
                            Name = oi.Item.Name
                        } : null
                    }).ToList() ?? new List<OrderItemDTO>()
                };

                Console.WriteLine($"[OrderController] DTO créé: OrderItems={dto.OrderItems?.Count ?? 0}");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderController] Exception: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"[OrderController] StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
        {
            try
            {
                Console.WriteLine($"[OrderController] Checkout appelé avec {dto.Items.Count} items");

                // Récupérer l'utilisateur connecté
                var user = await _userManager.FindByIdAsync("ffa98c22-aeb6-40bf-816b-00ff23e751b5");
                if (user == null)
                {
                    Console.WriteLine("[OrderController] Utilisateur non trouvé");
                    return Unauthorized("Utilisateur non connecté");
                }

                // Créer la commande
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    UserId = user.Id,
                    Status = "Pending",
                    ShippingAddress = dto.ShippingAddress,
                    City = dto.City,
                    PostalCode = dto.PostalCode,
                    Phone = dto.Phone,
                    Notes = dto.Notes,
                    TotalAmount = dto.TotalAmount
                };

                await _orderRepository.AddOrder(order);
                Console.WriteLine($"[OrderController] Commande créée avec ID {order.OrderId}");

                // Ajouter les OrderItems
                foreach (var item in dto.Items)
                {
                    await _orderItemRepository.AddOrderItem(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }
                Console.WriteLine($"[OrderController] {dto.Items.Count} OrderItems ajoutés");

                // Recharger la commande avec les OrderItems inclus
                var createdOrder = await _orderRepository.GetOrderById(order.OrderId);

                // Mapper vers DTO
                var orderDTO = new OrderDTO
                {
                    OrderId = createdOrder.OrderId,
                    OrderDate = createdOrder.OrderDate,
                    UserId = createdOrder.UserId,
                    Username = createdOrder.User?.UserName,
                    Status = createdOrder.Status,
                    ShippingAddress = createdOrder.ShippingAddress,
                    City = createdOrder.City,
                    PostalCode = createdOrder.PostalCode,
                    Phone = createdOrder.Phone,
                    Notes = createdOrder.Notes,
                    OrderItems = createdOrder.OrderItems?.Select(oi => new OrderItemDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ItemId = oi.ItemId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        ItemName = oi.Item?.Name ?? "Article",
                        Item = oi.Item != null ? new ItemDTO
                        {
                            ItemId = oi.Item.ItemId,
                            Name = oi.Item.Name
                        } : null
                    }).ToList() ?? new List<OrderItemDTO>()
                };

                Console.WriteLine($"[OrderController] Réponse DTO créée avec {orderDTO.OrderItems?.Count ?? 0} items");
                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderController] Exception Checkout: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"[OrderController] StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("user/orders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            // Récupérer l'ID utilisateur du JWT via les claims
            var userId = User.FindFirst("sub")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Impossible de récupérer l'ID utilisateur");

            var orders = await _orderRepository.GetOrdersByUser(userId);

            var ordersDto = orders?.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                UserId = o.UserId,
                Username = o.User?.UserName,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                City = o.City,
                PostalCode = o.PostalCode,
                Phone = o.Phone,
                Notes = o.Notes,
                OrderItems = o.OrderItems?.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ItemName = oi.Item?.Name,
                    Item = oi.Item != null ? new ItemDTO
                    {
                        ItemId = oi.Item.ItemId,
                        Name = oi.Item.Name,
                        Price = oi.Item.Price,
                        CategoryName = oi.Item.Category?.Name
                    } : null
                }).ToList() ?? new List<OrderItemDTO>()
            }).ToList() ?? new List<OrderDTO>();

            return Ok(ordersDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrdersByUserId(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUser(userId);

            var ordersDto = orders?.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                UserId = o.UserId,
                Username = o.User?.UserName,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                City = o.City,
                PostalCode = o.PostalCode,
                Phone = o.Phone,
                Notes = o.Notes,
                OrderItems = o.OrderItems?.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    ItemId = oi.ItemId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ItemName = oi.Item?.Name,
                    Item = oi.Item != null ? new ItemDTO
                    {
                        ItemId = oi.Item.ItemId,
                        Name = oi.Item.Name,
                        Price = oi.Item.Price,
                        CategoryName = oi.Item.Category?.Name
                    } : null
                }).ToList() ?? new List<OrderItemDTO>()
            }).ToList() ?? new List<OrderDTO>();

            return Ok(ordersDto);
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order.Status);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _orderRepository.UpdateOrder(order);
            return Ok();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null) return NotFound();

            order.Status = "Cancelled";
            await _orderRepository.UpdateOrder(order);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderCreateUpdateDTO orderDTO)
        {
            // Validate that userId is provided
            if (string.IsNullOrWhiteSpace(orderDTO.UserId))
                return BadRequest("UserId is required.");

            // Check if user exists
            var user = await _userManager.FindByIdAsync(orderDTO.UserId);
            if (user == null)
                return BadRequest($"User avec Id {orderDTO.UserId} n'existe pas.");

            // Get existing order
            var existingOrder = await _orderRepository.GetOrderById(id);
            if (existingOrder == null)
                return NotFound("Commande inexistante");

            // Update order
            existingOrder.OrderDate = orderDTO.OrderDate;
            existingOrder.UserId = orderDTO.UserId;

            var isUpdated = await _orderRepository.UpdateOrder(existingOrder);
            if (!isUpdated)
                return BadRequest("Impossible de mettre à jour la commande");

            // Reload user to get updated information
            var updatedUser = await _userManager.FindByIdAsync(existingOrder.UserId);

            // Return updated order as DTO
            var dto = new OrderDTO
            {
                OrderId = existingOrder.OrderId,
                OrderDate = existingOrder.OrderDate,
                UserId = existingOrder.UserId,
                Username = updatedUser?.UserName
            };

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var isDeleted = await _orderRepository.DeleteOrder(id);
            if (isDeleted)
                return NoContent();

            return NotFound("Commande inexistante");
        }
    }
}
