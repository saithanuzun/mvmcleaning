namespace mvmclean.backend.WebApp.Areas.Api.Models;

public class ServiceResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public int Duration { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? PostcodePrice { get; set; }
}
