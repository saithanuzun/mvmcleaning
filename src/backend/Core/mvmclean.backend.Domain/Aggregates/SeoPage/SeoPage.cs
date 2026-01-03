using mvmclean.backend.Domain.Aggregates.SeoPage.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Domain.Aggregates.SeoPage;

public class SeoPage : AggregateRoot
{
    // Core identity
    public string Slug { get; private set; }
    public string City { get; private set; }
    public string? Area { get; private set; }
    public string? ServiceType { get; private set; }

    public string MetaTitle { get; private set; }
    public string MetaDescription { get; private set; }
    public string H1Tag { get; private set; }

    public string Introduction { get; private set; }


    private readonly List<SeoPageContent> _contentBlocks = new();
    public IReadOnlyCollection<SeoPageContent> ContentBlocks => _contentBlocks.AsReadOnly();

    private readonly List<SeoPageFAQ> _faqs = new();
    public IReadOnlyCollection<SeoPageFAQ> FAQs => _faqs.AsReadOnly();

    private readonly List<SeoPageKeyword> _keywords = new();
    public IReadOnlyCollection<SeoPageKeyword> Keywords => _keywords.AsReadOnly();

    private SeoPage() { }

    // Factory
    public static SeoPage Create(string city, string area, string? serviceType)
    {
        var page = new SeoPage
        {
            Id = Guid.NewGuid(),
            City = city.Trim(),
            Area = area.Trim(),
            ServiceType = serviceType?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        page.Slug = GenerateSlug(page.City, page.Area, page.ServiceType);
        page.GenerateSeoMeta();
        page.GeneratePageContent();
        page.GenerateFaqs();
        page.GenerateKeywords();

        return page;
    }

    private static string GenerateSlug(string city, string? area, string? service)
    {
        return area is not null ? $"{city.ToLower().Replace(" ", "-")}/{area.ToLower().Replace(" ","-")}/{service.ToLower().Replace(" ", "-")}" 
            : $"{city.ToLower().Replace(" ", "-")}/{service.ToLower().Replace(" ", "-")}";
    }

    private void GenerateSeoMeta()
    {
        MetaTitle =
            $"{ServiceType} in {City} | BOOK ONLINE | Rated 4.9/5 · 10+ Years Experience  ";

        MetaDescription =
            $"Professional {ServiceType.ToLower()} services in {Area}, {City}. " +
            $"2000+ jobs completed. Same-day availability and free quotes.";

        H1Tag = $"{ServiceType} in {Area}, {City}";
    }

    // MAIN CONTENT
    private void GeneratePageContent()
    {
        Introduction =
            $"Looking for reliable {ServiceType.ToLower()} in {Area}, {City}? " +
            $"With over 10 years of experience and 2000+ completed jobs, " +
            $"we deliver high-quality results every time.";

        _contentBlocks.Clear();

        _contentBlocks.Add(SeoPageContent.Create(
            $"Professional {ServiceType} Services in {Area}",
            $"Our expert team provides professional {ServiceType.ToLower()} services across {Area}, {City}. " +
            $"We use advanced equipment and safe cleaning solutions to achieve outstanding results."
        ));

        _contentBlocks.Add(SeoPageContent.Create(
            $"Why Choose Us for {ServiceType}?",
            $"Customers in {Area} trust us for our fast response, competitive pricing, and proven results. " +
            $"All work is carried out by trained professionals with a satisfaction guarantee."
        ));

        _contentBlocks.Add(SeoPageContent.Create(
            $"{ServiceType} Prices in {Area}",
            $"Our pricing is clear and affordable. {ServiceType} costs in {Area} depend on room size and condition, " +
            $"with free, no-obligation quotes available."
        ));

        _contentBlocks.Add(SeoPageContent.Create(
            $"Areas We Cover Around {City}",
            $"We provide {ServiceType.ToLower()} services throughout {Area} and nearby locations in {City}. " +
            $"Contact us to check availability in your postcode."
        ));
    }

    private void GenerateFaqs()
    {
        _faqs.Clear();

        _faqs.AddRange(new[]
        {
            SeoPageFAQ.Create(
                $"How much does {ServiceType.ToLower()} cost in {Area}?",
                $"Prices vary depending on size and condition. Contact us for a free quote in {Area}."
            ),
            SeoPageFAQ.Create(
                $"How long does {ServiceType.ToLower()} take?",
                $"Most jobs in {Area} take between 1–3 hours, with drying times of 4–6 hours."
            ),
            SeoPageFAQ.Create(
                $"Do you cover all areas of {City}?",
                $"Yes, we serve {Area} and surrounding areas across {City}."
            ),
            SeoPageFAQ.Create(
                $"Can you remove stains?",
                $"Yes, we treat common stains including pet accidents, food spills, and general wear."
            )
        });
    }

    private void GenerateKeywords()
    {
        _keywords.Clear();

        var keywords = new List<string>
        {
            // Service + Location variations
            $"{ServiceType} {City}",
            $"{ServiceType} in {City}",
            $"{City} {ServiceType} services",
        
            // Professional qualifiers
            $"professional {ServiceType} {City}",
            $"certified {ServiceType} {City}",
            $"licensed {ServiceType} {City}",
        
            // "Best" variations (high commercial intent)
            $"best {ServiceType} {City}",
            $"top {ServiceType} {City}",
            $"affordable {ServiceType} {City}",
        
            // Service-specific with area
            $"{ServiceType} near {Area}",
            $"{Area} {ServiceType} company"
        };

        foreach (var keyword in keywords)
        {
            _keywords.Add(SeoPageKeyword.Create(keyword, ServiceType));
        }
    }

}
