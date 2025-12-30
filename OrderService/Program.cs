using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Consumers;
using OrderService.Data;
using OrderService.Entities;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(x =>
{ 
    x.AddConsumer<PaymentCompletedConsumer>();
    x.AddConsumer<PaymentFailedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("order-payment-events", e =>
        {
            e.ConfigureConsumer<PaymentCompletedConsumer>(context);
            e.ConfigureConsumer<PaymentFailedConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
