using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.Service.Entities;

public class Category : Entity
{
    public string Name { get; private set; }
    
    private readonly List<Service> _services = new();
    public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
    
    public bool IsActive { get; private set; }
    public int ServiceCount => _services.Count;
    
    private Category() { }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Category name is required");

        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new Exception("Category name is required");
            
        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
        
    }

    
    internal void AddService(Service service)
    {
        if (service == null)
            throw new Exception("Service cannot be null");
            
        _services.Add(service);
    }
    
    internal void RemoveService(Service service)
    {
        if (service == null)
            throw new Exception("Service cannot be null");
            
        _services.Remove(service);
    }
}
