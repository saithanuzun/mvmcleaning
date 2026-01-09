using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.ValueObjects;

public class SeoPageKeyword : ValueObject
{
    public string Keyword { get; private set; }
    public string Category { get; private set; }

    private SeoPageKeyword() { }

    public static SeoPageKeyword Create(string keyword, string category)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException("Keyword cannot be empty", nameof(keyword));
        
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        return new SeoPageKeyword
        {
            Keyword = keyword.Trim(),
            Category = category.Trim()
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Keyword;
        yield return Category;
    }

    public override string ToString() => Keyword;
}
