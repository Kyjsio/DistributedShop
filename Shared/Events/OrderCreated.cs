namespace Shared.Events
{
    public interface OrderCreated
    {
        int OrderId { get; }
        decimal TotalAmount { get; } // Kwota do zapłaty
        int UserId { get; }         // Kto kupił (opcjonalne)
    }
}