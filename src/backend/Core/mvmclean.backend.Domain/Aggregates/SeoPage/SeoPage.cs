using mvmclean.backend.Domain.Aggregates.SeoPage.Entities;
using mvmclean.backend.Domain.Core.BaseClasses;
using mvmclean.backend.Domain.Core.Interfaces;

namespace mvmclean.backend.Domain.Aggregates.SeoPage;

public class SeoPage : AggregateRoot
{
    // Core identity
    public string Slug { get; private set; }
    public string City { get; private set; }
    public string Area { get; private set; }
    public string ServiceType { get; private set; }

    // SEO Meta
    public string MetaTitle { get; private set; }
    public string MetaDescription { get; private set; }
    public string H1Tag { get; private set; }

    // Page Intro
    public string Introduction { get; private set; }

    // Statistics
    public int YearsServing { get; private set; } = 10;
    public int JobsCompleted { get; private set; } = 2000;
    public decimal AverageRating { get; private set; } = 4.9m;

    // Content Blocks (H2 + Paragraph)
    private readonly List<SeoPageContent> _contentBlocks = new();
    public IReadOnlyCollection<SeoPageContent> ContentBlocks => _contentBlocks.AsReadOnly();

    // FAQs
    private readonly List<SeoPageFAQ> _faqs = new();
    public IReadOnlyCollection<SeoPageFAQ> FAQs => _faqs.AsReadOnly();

    // Keywords
    private readonly List<SeoPageKeyword> _keywords = new();
    public IReadOnlyCollection<SeoPageKeyword> Keywords => _keywords.AsReadOnly();

    private SeoPage() { }

    // Factory
    public static SeoPage Create(
        string city,
        string area,
        string serviceType,
        int yearsServing,
        int jobsCompleted,
        decimal averageRating)
    {
        var page = new SeoPage
        {
            Id = Guid.NewGuid(),
            City = city.Trim(),
            Area = area.Trim(),
            ServiceType = serviceType.Trim(),
            YearsServing = yearsServing,
            JobsCompleted = jobsCompleted,
            AverageRating = averageRating,
            CreatedAt = DateTime.UtcNow
        };

        page.Slug = GenerateSlug(page.City, page.ServiceType);
        page.GenerateSeoMeta();
        page.GeneratePageContent();
        page.GenerateFaqs();
        page.GenerateKeywords();

        return page;
    }

    // Slug: /leicester/carpet-cleaning
    private static string GenerateSlug(string city, string service)
    {
        return $"{city.ToLower().Replace(" ", "-")}/{service.ToLower().Replace(" ", "-")}";
    }

    // META + H1
    private void GenerateSeoMeta()
    {
        MetaTitle =
            $"{ServiceType} in {City} | Rated {AverageRating}/5 · {YearsServing}+ Years Experience";

        MetaDescription =
            $"Professional {ServiceType.ToLower()} services in {Area}, {City}. " +
            $"{JobsCompleted}+ jobs completed. Same-day availability and free quotes.";

        H1Tag = $"{ServiceType} in {Area}, {City}";
    }

    // MAIN CONTENT
    private void GeneratePageContent()
    {
        Introduction =
            $"Looking for reliable {ServiceType.ToLower()} in {Area}, {City}? " +
            $"With over {YearsServing} years of experience and {JobsCompleted}+ completed jobs, " +
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

    // FAQ SECTION
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

        var keywords = new[]
        {
            $"{ServiceType.ToLower()} {Area}",
            $"{ServiceType.ToLower()} {City}",
            $"professional {ServiceType.ToLower()} {Area}",
            $"best {ServiceType.ToLower()} near me",
            $"{Area} {ServiceType.ToLower()} company"
        };

        foreach (var keyword in keywords)
        {
            _keywords.Add(SeoPageKeyword.Create(keyword, ServiceType));
        }
    }

    public void UpdateStatistics(int jobsCompleted, decimal averageRating)
    {
        JobsCompleted = jobsCompleted;
        AverageRating = averageRating;
        GenerateSeoMeta();
        GeneratePageContent();
    }
}
