namespace Shared.Events
{
    public interface OrderShipped
    {
        int OrderId { get; }
        DateTime ShippedAt { get; }
    }
}