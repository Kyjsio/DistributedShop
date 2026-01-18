namespace Shared.Events
{
    public record PaymentCompleted
    {
        public int OrderId { get; init; }
        public DateTime PaymentDate { get; init; }
        public decimal AmountEuro { get; init; }

        public List<PaymentItem> Items { get; init; } = new();
    }

    public record PaymentItem
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; }
    }
}