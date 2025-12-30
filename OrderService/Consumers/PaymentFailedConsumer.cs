using MassTransit;
using OrderService.Data;
using OrderService.Entities;
using Shared.Events;

namespace OrderService.Consumers
{
    public class PaymentFailedConsumer : IConsumer<PaymentFailed>
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(
            OrderDbContext context,
            ILogger<PaymentFailedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailed> context)
        {
            var msg = context.Message;

            var order = await _context.Orders.FindAsync(msg.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Zamowienie nie znalezione: {OrderId}", msg.OrderId);
                return;
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Zamowienie {OrderId} anulowane. Powod: {Reason}",msg.OrderId, msg.Reason);
        }
    }
}
