namespace mvmclean.backend.WebApp.Areas.Admin.Models;

public class CreateBookingFormModel
{
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public string Address { get; set; }
    public string Postcode { get; set; }
    public string ContractorId { get; set; }
    public string ScheduledDate { get; set; }
    public string ScheduledTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public decimal TotalAmount { get; set; }
    
    // Service selection
    public List<string> ServiceIds { get; set; } = new();
    public List<string> ServiceNames { get; set; } = new();
    public List<string> ServicePrices { get; set; } = new();
    public List<int> ServiceQuantities { get; set; } = new();
}