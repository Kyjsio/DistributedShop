using MassTransit;
using Shared.Events;

namespace PaymentService.Consumers
{
    public class PaymentServiceConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<PaymentServiceConsumer> _logger;

        public PaymentServiceConsumer(ILogger<PaymentServiceConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;

            _logger.LogInformation($" ID Zamówienia: {message.OrderId}");
            _logger.LogInformation($"Kwota: {message.TotalAmount} PLN");
            _logger.LogInformation($"ID Klienta: {message.UserId}");

            return Task.CompletedTask;
        }
    }
}