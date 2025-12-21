using ProductService.Entities;

namespace ProductService.Data
{
    public static class DbInitializer
    {
        public static void Seed(ProductDbContext context)
        {
            if (context.Products.Any()) return;

            var products = new List<Product>
            {
                new Product { Name = "Laptop Gamingowy", Description = "Super szybki", Price = 4500.00m, StockQuantity = 10 },
                new Product { Name = "Słuchawki BT", Description = "Długi czas pracy", Price = 199.99m, StockQuantity = 50 },
                new Product { Name = "Klawiatura Mech", Description = "Klikająca", Price = 350.00m, StockQuantity = 20 }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}