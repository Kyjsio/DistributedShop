using MassTransit; // Ważne: do komunikacji z RabbitMQ
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                UserId = 1,
                TotalAmount = 100m, 
                Status = OrderStatus.Created
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

        
            await _publishEndpoint.Publish<OrderCreated>(new
            {
                OrderId = newOrder.Id,
                TotalAmount = newOrder.TotalAmount,
                UserId = newOrder.UserId
            });

            return Ok(new { message = "Zamówienie przyjęte", orderId = newOrder.Id });
        }
    }
}