using MassTransit;
using PaymentService.Models;
using Shared.Events;
using System.Net.Http.Json;

namespace PaymentService.Consumers
{
    public class PaymentServiceConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<PaymentServiceConsumer> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentServiceConsumer(ILogger<PaymentServiceConsumer> logger, IHttpClientFactory httpClientFactory, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;

            var http = _httpClientFactory.CreateClient("fx");
            var fx = await http.GetFromJsonAsync<CurrencyApiResponse>("/v1/latest?from=PLN&to=EUR",context.CancellationToken);

            if (fx == null || !fx.Rates.TryGetValue("EUR", out var rate))
            {
                await _publishEndpoint.Publish<PaymentFailed>(new
                {
                    OrderId = message.OrderId,
                    Reason = "Nie udało się pobrać kursu wymiany PLN na EUR.",
                    FailedAt = DateTime.UtcNow
                }, context.CancellationToken);

                _logger.LogWarning("FX API failed for OrderId={OrderId}", message.OrderId);
                return;
            }

            var amountEur = Math.Round(message.TotalAmount * rate, 2);

            _logger.LogInformation("OrderId={OrderId} PLN={Pln} Rate={Rate} EUR={Eur}",
                message.OrderId, message.TotalAmount, rate, amountEur);

            var success = Random.Shared.NextDouble() < 0.8;

            if (success)
            {
                await _publishEndpoint.Publish<PaymentCompleted>(new
                {
                    OrderId = message.OrderId,
                    PaymentDate = DateTime.UtcNow,
                    AmountEuro = amountEur
                }, context.CancellationToken);

                _logger.LogInformation("Płatność dla OrderId={OrderId}", message.OrderId);
            }
            else
            {
                await _publishEndpoint.Publish<PaymentFailed>(new
                {
                    OrderId = message.OrderId,
                    FailedAt = DateTime.UtcNow,
                    Reason = "Symulacja: płatność odrzucona."
                }, context.CancellationToken);

                _logger.LogInformation("PPłatność nieudana OrderId={OrderId}", message.OrderId);
            }
        }
    }
}