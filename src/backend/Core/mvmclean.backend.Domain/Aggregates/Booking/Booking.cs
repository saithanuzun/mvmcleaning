using mvmclean.backend.Domain.Aggregates.Booking.Entities;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;
using mvmclean.backend.Domain.Aggregates.Booking.Events;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Booking;

public class Booking : Core.BaseClasses.AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; set; }
    public Guid? ContractorId { get; private set; }
    public Address ServiceAddress { get; private set; }
    public TimeSlot ScheduledSlot { get; private set; }
    public BookingStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }

    public IReadOnlyCollection<Guid> ServiceItemIds ;

    public Guid? PaymentId { get; private set; }
    public PaymentType PaymentType { get; set; }
    public Payment? Payment { get; private set; }

    private Booking(Customer customer, Address serviceAddress, TimeSlot scheduledSlot)
    {
        CustomerId = customer.Id;
        ServiceAddress = serviceAddress;
        ScheduledSlot = scheduledSlot;

        Status = BookingStatus.Pending;
        TotalPrice = Money.Zero();
    }


    public static Booking Create(Customer customer, Address serviceAddress, TimeSlot scheduledSlot)
    {
   
        var booking = new Booking(
            customer,
            serviceAddress,
            scheduledSlot
        );
        
        
        booking.AddDomainEvent(
            new BookingCreatedEvent(booking.Id, customer.Id)
        );

        return booking;
    }
    


    public void UpdateServiceAddress(Address newAddress)
    {
        ServiceAddress = newAddress;
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
