using mvmclean.backend.Domain.Aggregates.SeoPage.Enums;
using mvmclean.backend.Domain.Aggregates.SeoPage.ValueObjects;
using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage;

public class SeoPage : AggregateRoot
{
    // Route hierarchy
    public SeoPageLevel Level { get; private set; }
    
    // Core identity
    public string City { get; private set; }
    
    // Collections of value objects
    private readonly List<Area> _areas = new();
    public IReadOnlyCollection<Area> Areas => _areas.AsReadOnly();
    
    private readonly List<ServiceType> _services = new();
    public IReadOnlyCollection<ServiceType> Services => _services.AsReadOnly();
    
    // SEO Data properties
    public string Slug { get; private set; }
    public string MetaTitle { get; private set; }
    public string MetaDescription { get; private set; }
    public string H1Tag { get; private set; }
    public string Introduction { get; private set; }
    
    // Keywords
    private readonly List<SeoPageKeyword> _keywords = new();
    public IReadOnlyCollection<SeoPageKeyword> Keywords => _keywords.AsReadOnly();

    private SeoPage() { }

    /// <summary>
    /// Factory method - Create a SEO page for a city (required)
    /// Areas and services are optional and can be added via AddArea() and AddService()
    /// </summary>
    public static SeoPage CreateSeoPage(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        var page = new SeoPage
        {
            Id = Guid.NewGuid(),
            Level = SeoPageLevel.CityOnly,
            City = city.Trim(),
            Slug = city.ToLower().Replace(" ", "-"),
            CreatedAt = DateTime.UtcNow
        };

        page.UpdateContent();
        return page;
    }

    /// <summary>
    /// Add an area to the SEO page and update level/slug/content accordingly
    /// </summary>
    public void AddArea(string areaName)
    {
        if (string.IsNullOrWhiteSpace(areaName))
            throw new ArgumentException("Area name cannot be empty", nameof(areaName));

        var areaObj = Area.Create(areaName);
        
        // Check if area already exists
        if (_areas.Any(a => a.Name.Equals(areaName, StringComparison.OrdinalIgnoreCase)))
            return;

        _areas.Add(areaObj);
        UpdateLevelAndSlug();
        UpdateContent();
    }

    /// <summary>
    /// Add a service to the SEO page and update level/slug/content accordingly
    /// </summary>
    public void AddService(string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name cannot be empty", nameof(serviceName));

        var serviceObj = ServiceType.Create(serviceName);
        
        // Check if service already exists
        if (_services.Any(s => s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase)))
            return;

        _services.Add(serviceObj);
        UpdateLevelAndSlug();
        UpdateContent();
    }

    /// <summary>
    /// Helper to get area names as a comma-separated string
    /// </summary>
    public string GetAreasAsString() => string.Join(", ", _areas.Select(a => a.Name));

    /// <summary>
    /// Helper to get service names as a comma-separated string
    /// </summary>
    public string GetServicesAsString() => string.Join(", ", _services.Select(s => s.Name));

    /// <summary>
    /// Helper to get first area name (for route construction)
    /// </summary>
    public string? GetFirstAreaName() => _areas.FirstOrDefault()?.Name;

    /// <summary>
    /// Helper to get first service name (for route construction)
    /// </summary>
    public string? GetFirstServiceName() => _services.FirstOrDefault()?.Name;

    /// <summary>
    /// Update the page level based on current state of areas and services
    /// Level determination logic:
    /// - CityOnly: No areas or services
    /// - CityService: Has service(s) but no areas
    /// - CityArea: Has area(s) but no services
    /// - CityAreaService: Has both area(s) and service(s)
    /// </summary>
    private void UpdateLevelAndSlug()
    {
        var citySlug = City.ToLower().Replace(" ", "-");

        if (_areas.Count == 0 && _services.Count == 0)
        {
            Level = SeoPageLevel.CityOnly;
            Slug = citySlug;
        }
        else if (_areas.Count == 0 && _services.Count > 0)
        {
            Level = SeoPageLevel.CityService;
            var firstService = _services.First();
            Slug = $"{citySlug}/{firstService.Slug}";
        }
        else if (_areas.Count > 0 && _services.Count == 0)
        {
            Level = SeoPageLevel.CityArea;
            var firstArea = _areas.First();
            Slug = $"{citySlug}/{firstArea.Slug}";
        }
        else
        {
            Level = SeoPageLevel.CityAreaService;
            var firstArea = _areas.First();
            var firstService = _services.First();
            Slug = $"{citySlug}/{firstArea.Slug}/{firstService.Slug}";
        }
    }

    /// <summary>
    /// Update SEO content (MetaTitle, MetaDescription, H1Tag, Introduction) and keywords based on current state
    /// </summary>
    private void UpdateContent()
    {
        var areasString = GetAreasAsString();
        var servicesString = GetServicesAsString();

        switch (Level)
        {
            case SeoPageLevel.CityOnly:
                MetaTitle = $"{City} Cleaning Services | Professional Cleaners | Rated 4.9/5";
                MetaDescription = $"Professional cleaning services in {City}. Same-day availability, free quotes. Book online today!";
                H1Tag = $"Professional Cleaning Services in {City}";
                Introduction = $"Welcome to our {City} cleaning services. With 2000+ completed jobs and 10+ years of experience, we're your trusted cleaning partner.";
                break;

            case SeoPageLevel.CityService:
                var firstService = GetFirstServiceName();
                MetaTitle = $"{firstService} in {City} | Professional Cleaners | Rated 4.9/5";
                MetaDescription = $"Professional {firstService?.ToLower()} services in {City}. Same-day availability, free quotes. Book online today!";
                H1Tag = $"{firstService} in {City}";
                Introduction = $"Looking for reliable {firstService?.ToLower()} in {City}? We specialize in professional cleaning. With 10+ years of experience and 2000+ completed jobs, we deliver exceptional results.";
                break;

            case SeoPageLevel.CityArea:
                var firstArea = GetFirstAreaName();
                MetaTitle = $"Cleaning Services in {firstArea}, {City} | Book Online";
                MetaDescription = $"Professional cleaning services in {firstArea}, {City}. Same-day availability, free quotes. 2000+ jobs completed. Rated 4.9/5.";
                H1Tag = $"Cleaning Services in {firstArea}, {City}";
                Introduction = $"Looking for reliable cleaning services in {firstArea}, {City}? We specialize in professional cleaning. With 10+ years of experience and 2000+ completed jobs, we deliver exceptional results.";
                break;

            case SeoPageLevel.CityAreaService:
                var areaName = GetFirstAreaName();
                var serviceName = GetFirstServiceName();
                MetaTitle = $"{serviceName} in {areaName}, {City} | BOOK ONLINE | Rated 4.9/5";
                MetaDescription = $"Professional {serviceName?.ToLower()} services in {areaName}, {City}. 2000+ jobs completed. Same-day availability and free quotes.";
                H1Tag = $"{serviceName} in {areaName}, {City}";
                Introduction = $"Looking for reliable {serviceName?.ToLower()} in {areaName}, {City}? With over 10 years of experience and 2000+ completed jobs, we deliver high-quality results every time.";
                break;
        }

        GenerateKeywords();
    }

    private void GenerateKeywords()
    {
        _keywords.Clear();

        var keywords = new List<string>();
        var areasString = GetAreasAsString();
        var servicesString = GetServicesAsString();
        var firstAreaName = GetFirstAreaName();
        var firstServiceName = GetFirstServiceName();

        switch (Level)
        {
            case SeoPageLevel.CityOnly:
                // Keywords for city-level pages (e.g., /leicester)
                keywords = new List<string>
                {
                    $"cleaning services {City}",
                    $"professional cleaning {City}",
                    $"{City} cleaning services",
                    $"best cleaning {City}",
                    $"carpet cleaning {City}",
                    $"sofa cleaning {City}",
                    $"affordable cleaning {City}",
                    $"trusted cleaners {City}",
                    $"{servicesString} in {City}",
                    $"{City} {areasString}"
                };
                break;

            case SeoPageLevel.CityService:
                // Keywords for city + service pages (e.g., /leicester/carpet-cleaning)
                keywords = new List<string>
                {
                    $"{firstServiceName} {City}",
                    $"{firstServiceName} in {City}",
                    $"professional {firstServiceName} {City}",
                    $"{firstServiceName} services {City}",
                    $"best {firstServiceName} {City}",
                    $"affordable {firstServiceName} {City}",
                    $"{City} {firstServiceName}",
                    $"trusted {firstServiceName} {City}",
                    $"{firstServiceName} {areasString}",
                    $"certified {firstServiceName} {City}"
                };
                break;

            case SeoPageLevel.CityArea:
                // Keywords for city + area pages (e.g., /leicester/wigston)
                keywords = new List<string>
                {
                    $"cleaning services {firstAreaName}",
                    $"professional cleaning {firstAreaName}",
                    $"{firstAreaName} cleaning services",
                    $"best cleaning {firstAreaName}",
                    $"cleaning {firstAreaName} {City}",
                    $"{servicesString} in {firstAreaName}",
                    $"{City} {firstAreaName} cleaning",
                    $"affordable cleaning {firstAreaName}",
                    $"trusted cleaners {firstAreaName}",
                    $"{firstAreaName} {City} services"
                };
                break;

            case SeoPageLevel.CityAreaService:
                // Keywords for city + area + service pages (e.g., /leicester/wigston/carpet-cleaning)
                keywords = new List<string>
                {
                    $"{firstServiceName} {firstAreaName}",
                    $"{firstServiceName} in {firstAreaName}, {City}",
                    $"{firstAreaName} {firstServiceName}",
                    $"professional {firstServiceName} {firstAreaName}",
                    $"{firstServiceName} services {firstAreaName}",
                    $"best {firstServiceName} {City}",
                    $"certified {firstServiceName} {firstAreaName}",
                    $"affordable {firstServiceName} {firstAreaName}",
                    $"top {firstServiceName} {firstAreaName}",
                    $"{firstServiceName} {City}"
                };
                break;
        }

        foreach (var keyword in keywords)
        {
            _keywords.Add(SeoPageKeyword.Create(keyword, Level.ToString()));
        }
    }
}

