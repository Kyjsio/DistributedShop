using MassTransit;
using OrderService.Data;
using OrderService.Entities;
using Shared.Events;

namespace OrderService.Consumers
{
    public class PaymentCompletedConsumer :IConsumer<Shared.Events.PaymentCompleted>
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<PaymentCompletedConsumer> _logger;

        public PaymentCompletedConsumer(
            OrderDbContext context,
            ILogger<PaymentCompletedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompleted> context)
        {
            var msg = context.Message;

            var order = await _context.Orders.FindAsync(msg.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Zamowienie nie istnieje: {OrderId}", msg.OrderId);
                return;
            }

            order.Status = OrderStatus.Paid;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Zamowienie {OrderId} opłacone (Amount EUR: {Amount})",msg.OrderId, msg.AmountEuro);
        }
    }
}
