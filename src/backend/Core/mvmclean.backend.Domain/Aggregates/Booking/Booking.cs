using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Booking.Events;
\using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking;

public class Booking : Core.BaseClasses.AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Guid? ContractorId { get; private set; }
    public Address ServiceAddress { get; private set; }
    public TimeSlot ScheduledSlot { get; private set; }
    public BookingStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }

    private readonly List<BookingItem> _items = new();
    public IReadOnlyCollection<BookingItem> Items => _items.AsReadOnly();

    public Guid? PaymentId { get; private set; }
    public PaymentType PaymentType { get; set; }
    public Payment? Payment { get; private set; }

    private Booking(Guid customerId, Address serviceAddress, TimeSlot scheduledSlot)
    {
        CustomerId = customerId;
        ServiceAddress = serviceAddress;
        ScheduledSlot = scheduledSlot;

        Status = BookingStatus.Pending;
        TotalPrice = Money.Zero();
    }


    public static Booking Create(Guid customerId, Address serviceAddress, TimeSlot scheduledSlot)
    {
   
        var booking = new Booking(
            customerId,
            serviceAddress,
            scheduledSlot
        );

        booking.UpdatePostcodePricing(pricingService);
        booking.RecalculateTotalPrice();

        booking.AddDomainEvent(
            new BookingCreatedEvent(booking.Id, customerId)
        );

        return booking;
    }


    public void AddService(Guid serviceId, Money basePrice, int quantity = 1)
    {
        
        var item = new BookingItem
        {
            ServiceId = serviceId,
            AdjustedPrice = adjustedPrice, 
            Quantity = quantity
        };
        
        _items.Add(item);
        RecalculateTotalPrice();
    }

    public void AddServiceWithAdjustedPrice(Service service, Money adjustedPrice, int quantity = 1)
    {
        var item = new BookingItem
        {
            ServiceId = service.Id,
            AdjustedPrice = adjustedPrice,
            Quantity = quantity
        };
        
        _items.Add(item);
        RecalculateTotalPrice();
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = _items.Aggregate(
            Money.Create(0),
            (total, item) => total.Add(item.AdjustedPrice.Multiply(item.Quantity))
        );
    }

    public void UpdatePostcodePricing(IPricingService pricingService)
    {                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        foreach (var item in _items)
        {
            // Only recalculate if we have the base price
            if (item.BasePrice != null && item.BasePrice.Amount > 0)
            {
                item.AdjustedPrice = pricingService.CalculatePrice(item.BasePrice, ServiceAddress.Postcode);
            }
        }
        RecalculateTotalPrice();
    }                                                           

    public void UpdateServiceAddress(Address newAddress, IPricingService pricingService)
    {
        ServiceAddress = newAddress;
        UpdatePostcodePricing(pricingService);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignContractor(Contractor.Contractor contractor)
    {
        if (!contractor.IsAvailableAt(ScheduledSlot, ServiceAddress.Postcode))
        {
            throw new InvalidOperationException("Employee is not available for this time slot");
        }

        ContractorId = contractor.Id;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContractorAssignedEvent());
    }
    
    #region BookingFlow

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed");

        if (ContractorId == null)
            throw new InvalidOperationException("Cannot confirm booking without assigned employee");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookingConfirmedEvent(Id, ContractorId.Value));
    }

    public void Start()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be started");

        Status = BookingStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != BookingStatus.InProgress)
            throw new InvalidOperationException("Only in-progress bookings can be completed");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed bookings");

        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachPayment(Payment payment)
    {
        PaymentId = payment.Id;
        Payment = payment;
    }

    public void MarkAsFailed(string reason)
    {
        Status = BookingStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion Variables
    
}
