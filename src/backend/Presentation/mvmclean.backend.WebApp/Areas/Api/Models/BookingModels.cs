namespace mvmclean.backend.WebApp.Areas.Api.Models;

public class CreateBookingApiRequest
{
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string Address { get; set; }
    public string Postcode { get; set; }
    public Guid ContractorId { get; set; }
    public ScheduledSlotDto ScheduledSlot { get; set; }
    public List<ServiceItemDto> Services { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ScheduledSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class ServiceItemDto
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class VerifyPaymentRequest
{
    public Guid BookingId { get; set; }
    public string SessionId { get; set; }
}

public class CreateBookingResponse
{
    public Guid BookingId { get; set; }
    public string PaymentUrl { get; set; }
    public string Message { get; set; }
}

public class BookingDetailsResponse
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string Address { get; set; }
    public string Postcode { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public ScheduledSlotDto ScheduledSlot { get; set; }
    public List<ServiceItemDto> Services { get; set; }
    public DateTime CreatedAt { get; set; }
}
