namespace mvmclean.backend.WebApp.Areas.Api.Models;

public class AvailabilitySlot
{
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string DisplayTime { get; set; }
}

public class AvailabilityResponse
{
    public List<AvailabilitySlot> AvailableSlots { get; set; }
    public string Message { get; set; }
}
