using System.ComponentModel.DataAnnotations.Schema; // Ważne!

namespace OrderService.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        // NAPRAWA CS8618: Inicjalizujemy pustym stringiem
        public string ProductName { get; set; } = string.Empty;

        // NAPRAWA Decimal Warning: Precyzujemy typ kolumny
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}