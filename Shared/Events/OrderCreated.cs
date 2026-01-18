namespace Shared.Events
{
    public record OrderCreated
    {
        public int OrderId { get; init; }
        public decimal TotalAmount { get; init; }
        public int UserId { get; init; }

        public List<OrderItemMessage> Items { get; init; } = new();
    }

    public record OrderItemMessage
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; }
    }
}