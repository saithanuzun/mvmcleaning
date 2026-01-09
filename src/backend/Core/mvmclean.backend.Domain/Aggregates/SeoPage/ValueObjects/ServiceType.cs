using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.ValueObjects;

public class ServiceType : ValueObject
{
    public string Name { get; private set; }
    public string Slug { get; private set; }

    private ServiceType() { }

    public static ServiceType Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Service type name cannot be empty", nameof(name));

        var trimmedName = name.Trim();
        var slug = NormalizeToSlug(trimmedName);

        return new ServiceType 
        { 
            Name = trimmedName,
            Slug = slug
        };
    }

    private static string NormalizeToSlug(string text)
    {
        return text.ToLower().Replace(" ", "-");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Slug;
    }

    public override string ToString() => Name;
}
