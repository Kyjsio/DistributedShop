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

        public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
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

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                UserId = dto.UserId,
                Status = OrderStatus.Created,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),

                //na razie cena stała, zmienić później
                TotalAmount = 0m
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish<OrderCreated>(new
            {
                OrderId = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
            });

            return Ok(new { message = "Zamówienie przyjęte", orderId = order.Id });
        }


        //GET /api/orders
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

            return Ok(orders);
        }

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