using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Invoice.ValueObjects;

public class PaymentTerms : ValueObject
{
    public int DaysToPay { get; private set; }
    public decimal EarlyPaymentDiscountPercent { get; private set; }
    public int EarlyPaymentDiscountDays { get; private set; }
    
    public PaymentTerms(int daysToPay, decimal earlyPaymentDiscountPercent = 0, int earlyPaymentDiscountDays = 0)
    {
        DaysToPay = daysToPay;
        EarlyPaymentDiscountPercent = earlyPaymentDiscountPercent;
        EarlyPaymentDiscountDays = earlyPaymentDiscountDays;
    }
    
    public static PaymentTerms Net30 => new PaymentTerms(30);
    public static PaymentTerms Net15 => new PaymentTerms(15);
    public static PaymentTerms DueOnReceipt => new PaymentTerms(0);
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DaysToPay;
        yield return EarlyPaymentDiscountPercent;
        yield return EarlyPaymentDiscountDays;
    }
}
