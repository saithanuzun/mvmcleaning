using mvmclean.backend.Domain.Aggregates.Service.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Domain.Aggregates.Service;

public class Service : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Shortcut { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }

    private readonly List<PostcodePricing> _postcodePricings = new();
    public IReadOnlyCollection<PostcodePricing> PostcodePricings => _postcodePricings.AsReadOnly();
    public Category Category { get; private set; }
    public Guid CategoryId { get; private set; }
    public Money BasePrice { get; private set; } 
    

    private Service() { }

    public static Service Create(string name, string description, string shortcut, 
        decimal basePrice, TimeSpan estimatedDuration, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Service name is required");
        
        if (estimatedDuration <= TimeSpan.Zero)
            throw new Exception("Estimated duration must be positive");
        
        if (basePrice < 0)
            throw new Exception("Base price cannot be negative");
        
        if (category == null)
            throw new Exception("Category is required");

        return new Service
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Shortcut = shortcut?.Trim().ToUpper(),
            EstimatedDuration = estimatedDuration,
            Category = category,
            CategoryId = category.Id,
            BasePrice = Money.Create(basePrice), // Store base price as-is
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    
    public Money GetAdjustedPriceForPostcode(Postcode postcode)
    {
        var pricing = _postcodePricings.FirstOrDefault(p => p.Postcode.Area == postcode.Area);
        if (pricing == null)
            return BasePrice;
        return pricing.CalculateAdjustedPrice(BasePrice);
    }
    
    public void AddPostcodePricing(Postcode postcode, decimal multiplier, decimal fixedAdjustment)
    {
        var existingPricing = _postcodePricings
            .FirstOrDefault(p => p.Postcode == postcode);
    
        if (existingPricing != null)
        {
           existingPricing.UpdatePricing(multiplier,fixedAdjustment);
           return;
        }

        _postcodePricings.Add(PostcodePricing.Create(
            serviceId: Id,
            postcode: postcode,
            multiplier: multiplier,
            fixedAdjustment: fixedAdjustment
        ));
    
        UpdatedAt = DateTime.UtcNow;
    }
    

    public Money CalculatePriceForPostcode(Postcode postcode)
    {
        var pricing = _postcodePricings
            .FirstOrDefault(p => p.Postcode == postcode);

        if (pricing == null)
            return BasePrice;

        return pricing.CalculateAdjustedPrice(BasePrice);
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new Exception("Price cannot be negative");
        
        BasePrice = Money.Create(newPrice);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, TimeSpan estimatedDuration)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Service name is required");
            
        if (estimatedDuration <= TimeSpan.Zero)
            throw new Exception("Estimated duration must be positive");
        
        Name = name;
        Description = description;
        EstimatedDuration = estimatedDuration;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateShortcut(string newShortcut)
    {
        if (string.IsNullOrWhiteSpace(newShortcut))
            throw new Exception("Shortcut is required");
        
        Shortcut = newShortcut.Trim().ToUpper();
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ChangeCategory(Category newCategory)
    {
        if (newCategory == null)
            throw new Exception("Category is required");
            
        Category = newCategory;
        CategoryId = newCategory.Id;
        UpdatedAt = DateTime.UtcNow;
    }
}