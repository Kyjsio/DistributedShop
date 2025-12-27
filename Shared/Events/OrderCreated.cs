namespace Shared.Events
{
    public interface OrderCreated
    {
        int OrderId { get; }
        decimal TotalAmount { get; } 
        int UserId { get; }         
    }
}