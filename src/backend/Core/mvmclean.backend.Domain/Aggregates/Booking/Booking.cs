using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Booking.Events;
using mvmclean.backend.Domain.Aggregates.Service; // Add reference to Service aggregate
using mvmclean.backend.Domain.SharedKernel.ValueObjects;
using System.Collections.ObjectModel;

namespace mvmclean.backend.Domain.Aggregates.Booking;

public class Booking : Core.BaseClasses.AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public Guid? ContractorId { get; private set; }
    public Address ServiceAddress { get; private set; }
    public TimeSlot ScheduledSlot { get; private set; }
    public BookingStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }
    
    private readonly List<BookingItem> _serviceItems = new();

    public List<BookingItem> ServiceItems => new List<BookingItem>(_serviceItems);

    public Guid? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }

    private Booking(Customer customer, Address serviceAddress, TimeSlot scheduledSlot, List<BookingItem> serviceItems, Promotion.Promotion? promotion, Money totalPrice)
    {
        CustomerId = customer.Id;
        Customer = customer;
        ServiceAddress = serviceAddress;
        ScheduledSlot = scheduledSlot;
        _serviceItems = serviceItems;
        Status = BookingStatus.Pending;

        if (promotion == null)
        {
            TotalPrice = totalPrice;
        }
        else
        {
            TotalPrice = promotion.ApplyDiscount(totalPrice);
        }
    }

    protected Booking()
    {
        
    }

    public static Booking Create(Customer customer, Money totalPrice, Address serviceAddress, TimeSlot scheduledSlot, List<BookingItem> serviceItems, Promotion.Promotion? promotion)
    {
        var booking = new Booking(customer, serviceAddress, scheduledSlot, serviceItems, promotion, totalPrice);
        
        booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, customer.Id));
        return booking;
    }

    public void ApplyPromotion(Promotion.Promotion promotion)
    {
        TotalPrice = promotion.ApplyDiscount(TotalPrice);
    }

    
    public void UpdateServiceAddress(Address newAddress)
    {
        ServiceAddress = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
        
        UpdatedAt = DateTime.UtcNow;

        //AddDomainEvent(new ServiceAddressUpdatedEvent(Id, newAddress));
    }

    public void AssignContractor(Contractor.Contractor contractor)
    {
        if (contractor == null)
            throw new ArgumentNullException(nameof(contractor));

        if (!contractor.IsAvailableAt(ScheduledSlot, ServiceAddress.Postcode))
        {
            throw new InvalidOperationException("Contractor is not available for this time slot");
        }

        ContractorId = contractor.Id;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ContractorAssignedEvent(Id, contractor.Id));
    }

    #region BookingFlow

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed");

        if (ContractorId == null)
            throw new InvalidOperationException("Cannot confirm booking without assigned contractor");

        if (_serviceItems.Count == 0)
            throw new InvalidOperationException("Cannot confirm booking without services");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        //AddDomainEvent(new BookingConfirmedEvent(Id, ContractorId.Value, TotalPrice));
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

    public void Cancel(string? cancellationReason = null)
    {
        if (Status == BookingStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed bookings");

        var previousStatus = Status;
        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

    }
    

    #endregion
    
}