using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.ValueObjects;

public class Area : ValueObject
{
    public string Name { get; private set; }
    public string Slug { get; private set; }

    private Area() { }

    public static Area Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Area name cannot be empty", nameof(name));

        var trimmedName = name.Trim();
        var slug = NormalizeToSlug(trimmedName);

        return new Area 
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
