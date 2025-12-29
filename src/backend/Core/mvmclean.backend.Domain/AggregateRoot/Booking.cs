using mvmclean.backend.Domain.Entities;
using mvmclean.backend.Domain.Enums;
using mvmclean.backend.Domain.Events;
using mvmclean.backend.Domain.Services;
using mvmclean.backend.Domain.ValueObjects;

namespace mvmclean.backend.Domain.AggregateRoot;

public class Booking : Common.AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public Guid? ContractorId { get; private set; }
    public Contractor? Contractor { get; private set; }
    public Address ServiceAddress { get; private set; }
    public TimeSlot ScheduledSlot { get; private set; }
    public BookingStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }
    public Money BasePrice { get; private set; } // Price before postcode adjustments
    public Money PostcodeSurcharge { get; private set; } // Additional charge for postcode

    private readonly List<BookingItem> _items = new();
    public IReadOnlyCollection<BookingItem> Items => _items.AsReadOnly();

    public Guid? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }

    private Booking()
    {
    }

    public static Booking Create(Customer customer, Address serviceAddress, TimeSlot scheduledSlot, IPricingService pricingService = null)
    {
        var booking = new Booking
        {
            CustomerId = customer.Id,
            Customer = customer,
            ServiceAddress = serviceAddress,
            ScheduledSlot = scheduledSlot,
            Status = BookingStatus.Pending,
            TotalPrice = Money.Create(0),
            BasePrice = Money.Create(0),
            PostcodeSurcharge = Money.Create(0)
        };

        // Calculate initial postcode surcharge if pricing service provided
        if (pricingService != null)
        {
            booking.UpdatePostcodePricing(pricingService);
        }

        booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, customer.Id));
        return booking;
    }

    public void AddService(Service service, Money basePrice, IPricingService pricingService, int quantity = 1)
    {
        var adjustedPrice = pricingService.CalculatePrice(basePrice, ServiceAddress.Postcode);
        
        var item = new BookingItem
        {
            ServiceId = service.Id,
            Service = service,
            BasePrice = basePrice, // Store base price
            AdjustedPrice = adjustedPrice, // Store postcode-adjusted price
            Quantity = quantity
        };
        
        _items.Add(item);
        RecalculateTotalPrice();
    }

    // Alternative method if you want to calculate price outside
    public void AddServiceWithAdjustedPrice(Service service, Money adjustedPrice, int quantity = 1)
    {
        var item = new BookingItem
        {
            ServiceId = service.Id,
            Service = service,
            BasePrice = adjustedPrice, // In this case, price is already adjusted
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
        
        // Calculate base price and surcharge
        BasePrice = _items.Aggregate(
            Money.Create(0),
            (total, item) => total.Add(item.BasePrice.Multiply(item.Quantity))
        );
        
        PostcodeSurcharge = TotalPrice.Subtract(BasePrice);
    }

    // Update pricing if postcode changes or service list changes
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

    // Method to change service address with pricing update
    public void UpdateServiceAddress(Address newAddress, IPricingService pricingService)
    {
        ServiceAddress = newAddress;
        UpdatePostcodePricing(pricingService);
        UpdatedAt = DateTime.UtcNow;
    }

    // Rest of the methods remain the same...
    public void AssignEmployee(Contractor contractor)
    {
        if (!contractor.IsAvailableAt(ScheduledSlot, ServiceAddress.Postcode))
        {
            throw new InvalidOperationException("Employee is not available for this time slot");
        }

        ContractorId = contractor.Id;
        Contractor = contractor;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EmployeeAssignedEvent(Id, contractor.Id));
    }

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
}


public class BookingItem
{
    public Guid ServiceId { get; set; }
    public Service Service { get; set; }
    public Money BasePrice { get; set; } // Original price without postcode adjustment
    public Money AdjustedPrice { get; set; } // Price after postcode adjustment
    public int Quantity { get; set; }
}
