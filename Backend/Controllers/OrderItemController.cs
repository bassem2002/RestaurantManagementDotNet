using Backend.DTOs;
using Backend.Models;
using Backend.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderItemController(IOrderItemRepository orderItemRepository, UserManager<ApplicationUser> userManager)
        {
            _orderItemRepository = orderItemRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrdersItem()
        {
            var orderItems = await _orderItemRepository.GetOrderItems();
            return Ok(orderItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderItemById(int id)
        {
            var existingOrderItem = await _orderItemRepository.GetOrderItemById(id);
            if (existingOrderItem == null)
                return NotFound("OrderItem inexistant");
            return Ok(existingOrderItem);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderItem([FromBody] OrderItemCreateUpdateDTO orderItemDTO)
        {
            try
            {
                var orderItem = new OrderItem
                {
                    Quantity = orderItemDTO.Quantity,
                    UnitPrice = orderItemDTO.UnitPrice,
                    OrderId = orderItemDTO.OrderId,
                    ItemId = orderItemDTO.ItemId
                };

                var newOrderItem = await _orderItemRepository.AddOrderItem(orderItem);

                // Build response DTO with related entities
                var dto = new OrderItemDTO
                {
                    OrderItemId = newOrderItem.OrderItemId,
                    Quantity = newOrderItem.Quantity,
                    UnitPrice = newOrderItem.UnitPrice,
                    OrderId = newOrderItem.OrderId,
                    ItemId = newOrderItem.ItemId,
                    Order = newOrderItem.Order != null ? new OrderDTO
                    {
                        OrderId = newOrderItem.Order.OrderId,
                        OrderDate = newOrderItem.Order.OrderDate,
                        UserId = newOrderItem.Order.UserId,
                        Username = newOrderItem.Order.User?.UserName
                    } : null,
                    Item = newOrderItem.Item != null ? new ItemDTO
                    {
                        ItemId = newOrderItem.Item.ItemId,
                        Name = newOrderItem.Item.Name,
                        Price = newOrderItem.Item.Price,
                        CategoryName = newOrderItem.Item.Category?.Name
                    } : null
                };

                return CreatedAtAction(nameof(GetOrderItemById),
                    new { id = newOrderItem.OrderItemId },
                    dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur lors de la création de l'OrderItem", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] OrderItemCreateUpdateDTO orderItemDTO)
        {
            try
            {
                var existingOrderItem = await _orderItemRepository.GetOrderItemById(id);
                if (existingOrderItem == null)
                    return NotFound("OrderItem inexistant");

                existingOrderItem.Quantity = orderItemDTO.Quantity;
                existingOrderItem.UnitPrice = orderItemDTO.UnitPrice;

                var isUpdated = await _orderItemRepository.UpdateOrderItem(existingOrderItem);
                if (!isUpdated)
                    return BadRequest("Impossible de mettre à jour l'OrderItem");

                // Return updated OrderItem as DTO
                var dto = new OrderItemDTO
                {
                    OrderItemId = existingOrderItem.OrderItemId,
                    Quantity = existingOrderItem.Quantity,
                    UnitPrice = existingOrderItem.UnitPrice,
                    OrderId = existingOrderItem.OrderId,
                    ItemId = existingOrderItem.ItemId,
                    Order = existingOrderItem.Order != null ? new OrderDTO
                    {
                        OrderId = existingOrderItem.Order.OrderId,
                        OrderDate = existingOrderItem.Order.OrderDate,
                        UserId = existingOrderItem.Order.UserId,
                        Username = existingOrderItem.Order.User?.UserName
                    } : null,
                    Item = existingOrderItem.Item != null ? new ItemDTO
                    {
                        ItemId = existingOrderItem.Item.ItemId,
                        Name = existingOrderItem.Item.Name,
                        Price = existingOrderItem.Item.Price,
                        CategoryName = existingOrderItem.Item.Category?.Name
                    } : null
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur lors de la mise à jour", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            try
            {
                var isDeleted = await _orderItemRepository.DeleteOrderItem(id);
                if (isDeleted)
                    return NoContent();

                return NotFound("OrderItem inexistant");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur lors de la suppression", Error = ex.Message });
            }
        }
    }
}
