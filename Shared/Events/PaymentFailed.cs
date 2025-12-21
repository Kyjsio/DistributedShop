namespace Shared.Events
{
    public interface PaymentFailed
    {
        int OrderId { get; }
        string Reason { get; }
    }
}