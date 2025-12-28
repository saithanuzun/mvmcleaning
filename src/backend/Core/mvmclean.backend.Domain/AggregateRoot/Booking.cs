using mvmclean.backend.Domain.Entities;
using mvmclean.backend.Domain.Enums;
using mvmclean.backend.Domain.Events;
using mvmclean.backend.Domain.ValueObjects;

namespace mvmclean.backend.Domain.AggregateRoot;

public class Booking : Common.AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public Employee? Employee { get; private set; }
    public Address ServiceAddress { get; private set; }
    public TimeSlot ScheduledSlot { get; private set; }
    public BookingStatus Status { get; private set; }
    public Money TotalPrice { get; private set; }

    private readonly List<BookingItem> _items = new();
    public IReadOnlyCollection<BookingItem> Items => _items.AsReadOnly();

    public Guid? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }

    private Booking()
    {
    }

    public static Booking Create(Customer customer, Address serviceAddress, TimeSlot scheduledSlot)
    {
        var booking = new Booking
        {
            CustomerId = customer.Id,
            Customer = customer,
            ServiceAddress = serviceAddress,
            ScheduledSlot = scheduledSlot,
            Status = BookingStatus.Pending,
            TotalPrice = Money.Create(0)
        };

        booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, customer.Id));
        return booking;
    }

    public void AddService(Service service, Money price, int quantity = 1)
    {
        var item = new BookingItem
        {
            ServiceId = service.Id,
            Service = service,
            Price = price,
            Quantity = quantity
        };
        _items.Add(item);
        RecalculateTotalPrice();
    }

    public void AddPackage(Package package, Money packagePrice)
    {
        foreach (var packageService in package.Services)
        {
            var item = new BookingItem
            {
                ServiceId = packageService.ServiceId,
                Service = packageService.Service,
                Price = packagePrice,
                Quantity = packageService.Quantity
            };
            _items.Add(item);
        }

        RecalculateTotalPrice();
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = _items.Aggregate(Money.Create(0),
            (total, item) => total.Add(item.Price.Multiply(item.Quantity)));
    }

    public void AssignEmployee(Employee employee)
    {
        if (!employee.IsAvailableAt(ScheduledSlot, ServiceAddress.Postcode))
        {
            throw new InvalidOperationException("Employee is not available for this time slot");
        }

        EmployeeId = employee.Id;
        Employee = employee;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EmployeeAssignedEvent(Id, employee.Id));
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Only pending bookings can be confirmed");

        if (EmployeeId == null)
            throw new InvalidOperationException("Cannot confirm booking without assigned employee");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookingConfirmedEvent(Id, EmployeeId.Value));
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
    public Money Price { get; set; }
    public int Quantity { get; set; }
}