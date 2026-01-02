namespace mvmclean.backend.Domain.Aggregates.Service;

public interface IServiceRepository
{
    Task<List<Service>> GetAllServicesByPostcode(string postcode);
    Task<Service?> GetServiceByPostcode(string postcode);
}