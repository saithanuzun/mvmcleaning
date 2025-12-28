using mvmclean.backend.Domain.Common;

namespace mvmclean.backend.Domain.Entities;

public class Package : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    private readonly List<PackageService> _services = new();
    public IReadOnlyCollection<PackageService> Services => _services.AsReadOnly();

    private Package() { }

    public static Package Create(string name, string description)
    {
        return new Package
        {
            Name = name,
            Description = description,
            IsActive = true
        };
    }

    public void AddService(Service service, int quantity)
    {
        var packageService = new PackageService
        {
            ServiceId = service.Id,
            Service = service,
            Quantity = quantity
        };
        _services.Add(packageService);
    }

    public void RemoveService(Guid serviceId)
    {
        var service = _services.FirstOrDefault(s => s.ServiceId == serviceId);
        if (service != null) _services.Remove(service);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

public class PackageService
{
    public Guid ServiceId { get; set; }
    public Service Service { get; set; }
    public int Quantity { get; set; }
}
