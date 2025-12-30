namespace Shared.Events
{
    public interface PaymentCompleted
    {
        int OrderId { get; }
        DateTime PaymentDate { get; }
        decimal AmountEuro { get; }
    }
}