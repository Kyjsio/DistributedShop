using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using Shared.Events;

namespace ProductService.Consumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompleted>
{
    private readonly ProductDbContext _db;

    public PaymentCompletedConsumer(ProductDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<PaymentCompleted> context)
    {
        var msg = context.Message;

        foreach (var item in msg.Items)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product == null) continue; // albo: throw, zależy jak chcesz

            product.StockQuantity -= item.Quantity;
            if (product.StockQuantity < 0) product.StockQuantity = 0; // prosto, na start
        }

        await _db.SaveChangesAsync();
    }
}
