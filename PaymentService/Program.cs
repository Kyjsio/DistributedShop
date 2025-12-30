using MassTransit;
using PaymentService.Consumers;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>

{ x.AddConsumer<PaymentServiceConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("payment-service", e =>
        {
            e.ConfigureConsumer<PaymentServiceConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//api do pobierania kursu walut
builder.Services.AddHttpClient("fx", c =>
{
    c.BaseAddress = new Uri("https://api.frankfurter.dev");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
