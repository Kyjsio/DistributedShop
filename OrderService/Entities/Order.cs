using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Created;
        public List<OrderItem> Items { get; set; } = new();
    }
}