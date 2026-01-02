using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.DTOs;
using OrderService.Entities;
using Shared.Events; 

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _httpClientFactory = httpClientFactory;
        }


        //POST /api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (dto?.Items == null || dto.Items.Count == 0)
            {
                return BadRequest("Zamówienie musi mieć przynajmniej jeden produkt!");
            }

            if (dto.Items.Any(i => i.Quantity <= 0))
            {
                return BadRequest("Ilość produktu musi być większa niż zero!");
            }
            var productClient = _httpClientFactory.CreateClient("ProductClient");

            decimal totalAmount = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in dto.Items)
            {
                var product = await productClient.GetFromJsonAsync<ProductDto>($"api/product/{itemDto.ProductId}");

                if (product == null)
                {
                    return BadRequest($"Produkt o ID {itemDto.ProductId} nie istnieje w systemie ProductService.");
                }

                decimal itemTotal = product.Price * itemDto.Quantity;
                totalAmount += itemTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                });
            }

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                UserId = dto.UserId,
                Status = OrderStatus.Created,
                Items = orderItems,
                TotalAmount = totalAmount 
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Zamówienie przyjęte", orderId = order.Id, totalAmount = order.TotalAmount });
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayForOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            if (order.Status == OrderStatus.Paid) return BadRequest("Już opłacone");

            await _publishEndpoint.Publish<OrderCreated>(new
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId
            });

            return Accepted(new { message = "Płatność rozpoczęta..." });
        }

        //GET /api/orders
        //[HttpGet]
        //public async Task<IActionResult> GetOrders()
        //{
        //    var orders = await _context.Orders
        //        .Include(o => o.Items)
        //        .ToListAsync();
        //    return Ok(orders);
        //}

        //GET /api/orders/{id}
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if(order ==null)
                return NotFound();

            return Ok(order);
        }
    }
}