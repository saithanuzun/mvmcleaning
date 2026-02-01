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

    private SeoPage()
    {
    }

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
        if (string.IsNullOrWhiteSpace(City))
        {
            throw new InvalidOperationException("City must be set before updating content.");
        }

        var serviceName = GetFirstServiceName() ?? "Carpet & Sofa Cleaning";
        var areaName = GetFirstAreaName() ?? City;
        var serviceKeyword = serviceName.ToLowerInvariant();
        var locationKeyword = areaName != City ? $"{areaName}, {City}" : City;

        // Common SEO phrases for carpet and sofa cleaning industry
        const string rating = "Rated 4.9/5 Stars";
        const string experience = "10+ Years Experience";
        const string jobsCompleted = "2000+ Satisfied Customers";
        const string sameDay = "Same-Day Service Available";
        const string ecoFriendly = "Eco-Friendly Cleaning Solutions";
        const string freeQuote = "Free, No-Obligation Quotes";

        switch (Level)
        {
            case SeoPageLevel.CityOnly:
                MetaTitle = $"{City} Carpet & Sofa Cleaning | Professional {serviceName} Services | {rating}";
                MetaDescription =
                    $"Expert carpet cleaning and sofa cleaning services in {City}. Deep clean, stain removal, and odor elimination. {sameDay} with {experience}. {freeQuote}.";
                H1Tag = $"Professional Carpet & Sofa Cleaning Services in {City}";
                Introduction =
                    $"Welcome to the leading carpet and sofa cleaning company in {City}. Our certified technicians use state-of-the-art equipment and eco-friendly cleaning solutions to revitalize your carpets, rugs, and upholstery. With {experience} and {jobsCompleted}, we guarantee exceptional results and complete satisfaction.";
                break;

            case SeoPageLevel.CityService:
                MetaTitle = $"Best {serviceName} in {City} | Affordable & Professional Services | {rating}";
                MetaDescription =
                    $"Top-rated {serviceKeyword} services in {City}. Specialized cleaning for carpets, sofas, and upholstery. {sameDay}, {ecoFriendly}. {freeQuote} and satisfaction guaranteed.";
                H1Tag = $"Professional {serviceName} Services in {City}";
                Introduction =
                    $"Searching for the most reliable {serviceKeyword} company in {City}? We specialize in comprehensive {serviceKeyword} using advanced steam cleaning and dry cleaning methods. Our trained professionals provide thorough deep cleaning, stain removal, and fabric protection services to extend the life of your furnishings.";
                break;

            case SeoPageLevel.CityArea:
                MetaTitle = $"Carpet & Sofa Cleaning in {areaName}, {City} | Local Cleaners | {rating}";
                MetaDescription =
                    $"Local carpet and sofa cleaners serving {areaName}, {City}. Residential & commercial cleaning. Pet stain removal, allergy relief cleaning. {sameDay} service.";
                H1Tag = $"Carpet & Sofa Cleaning Services in {areaName}, {City}";
                Introduction =
                    $"As your local carpet and sofa cleaning experts in {areaName}, {City}, we understand the specific needs of homes and businesses in our community. We offer personalized cleaning solutions including pet odor removal, allergy-friendly cleaning, and commercial maintenance programs. Serving {areaName} with pride for over 10 years.";
                break;

            case SeoPageLevel.CityAreaService:
                MetaTitle = $"{serviceName} in {areaName}, {City} | Fast, Reliable Service | {rating}";
                MetaDescription =
                    $"Expert {serviceKeyword} in {areaName}, {City}. Immediate response, professional results. {ecoFriendly}, pet-safe solutions. Call now for {freeQuote}!";
                H1Tag = $"{serviceName} Services in {areaName}, {City}";
                Introduction =
                    $"When you need professional {serviceKeyword} services in {areaName}, {City}, our local team is ready to help. We use truck-mounted steam cleaning systems and eco-certified products to deliver superior cleaning results while being safe for children, pets, and the environment. Emergency flood restoration and stain treatment available.";
                break;
        }

        GenerateKeywords();
    }

    private void GenerateKeywords()
    {
        _keywords.Clear();

        var firstAreaName = GetFirstAreaName() ?? City;
        var firstServiceName = GetFirstServiceName() ?? "Carpet & Sofa Cleaning";
        var serviceLower = firstServiceName.ToLowerInvariant();
        var cityLower = City.ToLowerInvariant();
        var areaLower = firstAreaName.ToLowerInvariant();

        List<string> keywords = new List<string>();

        // Core keyword groups for carpet and sofa cleaning industry
        var coreServices = new List<string>
        {
            "carpet cleaning",
            "sofa cleaning",
            "upholstery cleaning",
            "rug cleaning",
            "furniture cleaning",
            "deep cleaning",
            "steam cleaning",
            "dry cleaning",
            "professional cleaning"
        };

        var problemKeywords = new List<string>
        {
            "stain removal",
            "pet stain removal",
            "odor elimination",
            "mold removal",
            "allergy cleaning",
            "deep clean",
            "flood restoration",
            "emergency cleaning",
            "spot cleaning"
        };

        var qualifierKeywords = new List<string>
        {
            "professional",
            "best",
            "affordable",
            "cheap",
            "local",
            "certified",
            "trusted",
            "experienced",
            "reliable",
            "same day",
            "emergency",
            "eco friendly",
            "green cleaning",
            "commercial",
            "residential"
        };

        var intentKeywords = new List<string>
        {
            "services",
            "company",
            "near me",
            "prices",
            "cost",
            "quote",
            "reviews",
            "how to clean",
            "cleaning tips",
            "before and after"
        };

        var locationKeywords = new List<string>
        {
            City,
            firstAreaName,
            $"{firstAreaName} {City}",
            $"{City} area",
            $"{City} and surrounding areas"
        };

        switch (Level)
        {
            case SeoPageLevel.CityOnly:
                // City-wide keywords - broad match
                keywords = GenerateKeywordCombinations(
                    coreServices,
                    problemKeywords,
                    qualifierKeywords,
                    intentKeywords,
                    locationKeywords.Where(l => l.Contains(City)).ToList()
                );

                // Add specific high-value keywords
                keywords.AddRange(new[]
                {
                    $"professional carpet cleaners {City}",
                    $"emergency sofa cleaning {City}",
                    $"carpet cleaning companies {City}",
                    $"best upholstery cleaning {City}",
                    $"affordable rug cleaning {City}",
                    $"same day cleaning service {City}",
                    $"pet stain removal specialists {City}",
                    $"eco friendly cleaning {City}",
                    $"commercial carpet cleaning {City}",
                    $"move in move out cleaning {City}",
                    $"professional steam cleaning {City}",
                    $"dry carpet cleaning {City}",
                    $"flood damage restoration {City}",
                    $"allergy relief cleaning {City}",
                    $"fabric protection service {City}"
                });
                break;

            case SeoPageLevel.CityService:
                // Service-specific keywords in city
                var serviceVariations = GetServiceKeywordVariations(firstServiceName);

                keywords = GenerateServiceSpecificKeywords(
                    serviceVariations,
                    qualifierKeywords,
                    problemKeywords,
                    intentKeywords,
                    locationKeywords,
                    cityLower
                );

                // Add geo-modified service keywords
                keywords.AddRange(new[]
                {
                    $"{firstServiceName} cost {City}",
                    $"{firstServiceName} prices {City}",
                    $"{firstServiceName} near me {City}",
                    $"{firstServiceName} companies {City}",
                    $"{firstServiceName} reviews {City}",
                    $"{firstServiceName} before and after {City}",
                    $"how much does {firstServiceName} cost in {City}",
                    $"best rated {firstServiceName} {City}",
                    $"licensed {firstServiceName} {City}",
                    $"emergency {firstServiceName} {City}"
                });
                break;

            case SeoPageLevel.CityArea:
                // Area-specific keywords within city
                keywords = GenerateAreaSpecificKeywords(
                    coreServices,
                    problemKeywords,
                    qualifierKeywords,
                    areaLower,
                    cityLower
                );

                // Add neighborhood-specific keywords
                keywords.AddRange(new[]
                {
                    $"local cleaners in {firstAreaName} {City}",
                    $"{firstAreaName} carpet cleaning companies",
                    $"{firstAreaName} sofa cleaning specialists",
                    $"cleaning services near {firstAreaName}",
                    $"best cleaners in {firstAreaName} {City}",
                    $"same day cleaning {firstAreaName}",
                    $"emergency cleaning {firstAreaName} {City}",
                    $"residential cleaning {firstAreaName}",
                    $"commercial cleaning {firstAreaName}",
                    $"move out cleaning {firstAreaName} {City}"
                });
                break;

            case SeoPageLevel.CityAreaService:
                // Hyper-local service keywords
                keywords = GenerateHyperLocalKeywords(
                    firstServiceName,
                    serviceLower,
                    areaLower,
                    cityLower,
                    qualifierKeywords,
                    problemKeywords
                );

                // Add specific transactional keywords
                keywords.AddRange(new[]
                {
                    $"{firstServiceName} quote {firstAreaName}",
                    $"{firstServiceName} cost {firstAreaName} {City}",
                    $"{firstServiceName} emergency service {firstAreaName}",
                    $"{firstServiceName} same day {firstAreaName}",
                    $"{firstServiceName} reviews {firstAreaName}",
                    $"book {firstServiceName} {firstAreaName}",
                    $"schedule {firstServiceName} {firstAreaName}",
                    $"{firstServiceName} appointment {firstAreaName}",
                    $"urgent {firstServiceName} {firstAreaName}",
                    $"{firstServiceName} professionals {firstAreaName}"
                });
                break;
        }

        // Remove duplicates and sort by search volume potential
        keywords = keywords
            .Distinct()
            .Select(k => k.Trim().ToLowerInvariant())
            .OrderByDescending(k => CalculateKeywordPriority(k))
            .Take(50) // Limit to top 50 most relevant keywords
            .ToList();

        foreach (var keyword in keywords)
        {
            _keywords.Add(SeoPageKeyword.Create(keyword, Level.ToString()));
        }
    }

    private List<string> GenerateKeywordCombinations(
        List<string> services,
        List<string> problems,
        List<string> qualifiers,
        List<string> intents,
        List<string> locations)
    {
        var combinations = new List<string>();

        foreach (var location in locations)
        {
            // Service + Location
            foreach (var service in services)
            {
                combinations.Add($"{service} {location}");

                // Service + Qualifier + Location
                foreach (var qualifier in qualifiers.Take(5))
                {
                    combinations.Add($"{qualifier} {service} {location}");
                }

                // Service + Intent + Location
                foreach (var intent in intents.Take(3))
                {
                    combinations.Add($"{service} {intent} {location}");
                }
            }

            // Problem + Location
            foreach (var problem in problems)
            {
                combinations.Add($"{problem} {location}");

                // Problem + Qualifier + Location
                foreach (var qualifier in qualifiers.Take(3))
                {
                    combinations.Add($"{qualifier} {problem} {location}");
                }
            }
        }

        return combinations;
    }

    private List<string> GetServiceKeywordVariations(string serviceName)
    {
        var variations = new List<string> { serviceName.ToLowerInvariant() };

        // Common variations for carpet and sofa cleaning services
        if (serviceName.Contains("Carpet", StringComparison.OrdinalIgnoreCase))
        {
            variations.AddRange(new[]
            {
                "carpet cleaners",
                "carpet cleaning service",
                "carpet cleaning company",
                "professional carpet cleaning",
                "carpet steam cleaning",
                "carpet deep cleaning"
            });
        }

        if (serviceName.Contains("Sofa", StringComparison.OrdinalIgnoreCase) ||
            serviceName.Contains("Upholstery", StringComparison.OrdinalIgnoreCase))
        {
            variations.AddRange(new[]
            {
                "sofa cleaners",
                "upholstery cleaning",
                "furniture cleaning",
                "couch cleaning",
                "settee cleaning",
                "sofa steam cleaning",
                "chair cleaning"
            });
        }

        return variations;
    }

    private List<string> GenerateServiceSpecificKeywords(
        List<string> serviceVariations,
        List<string> qualifiers,
        List<string> problems,
        List<string> intents,
        List<string> locations,
        string cityLower)
    {
        var keywords = new List<string>();

        foreach (var service in serviceVariations)
        {
            foreach (var location in locations)
            {
                keywords.Add($"{service} {location}");

                // Service + Qualifier + Location
                foreach (var qualifier in qualifiers.Take(7))
                {
                    keywords.Add($"{qualifier} {service} {location}");
                }

                // Service + Problem + Location
                foreach (var problem in problems.Take(5))
                {
                    keywords.Add($"{service} {problem} {location}");
                }

                // Service + Intent + Location
                foreach (var intent in intents.Take(5))
                {
                    keywords.Add($"{service} {intent} {location}");
                }
            }
        }

        return keywords;
    }

    private List<string> GenerateAreaSpecificKeywords(
        List<string> services,
        List<string> problems,
        List<string> qualifiers,
        string areaLower,
        string cityLower)
    {
        var keywords = new List<string>();

        var locationVariations = new List<string>
        {
            areaLower,
            $"{areaLower} {cityLower}",
            $"{cityLower} {areaLower}"
        };

        foreach (var location in locationVariations)
        {
            foreach (var service in services)
            {
                keywords.Add($"{service} {location}");

                // Service + Qualifier + Location
                foreach (var qualifier in qualifiers.Take(5))
                {
                    keywords.Add($"{qualifier} {service} {location}");
                }
            }

            // Problem + Location
            foreach (var problem in problems)
            {
                keywords.Add($"{problem} {location}");
            }
        }

        return keywords;
    }

    private List<string> GenerateHyperLocalKeywords(
        string serviceName,
        string serviceLower,
        string areaLower,
        string cityLower,
        List<string> qualifiers,
        List<string> problems)
    {
        var keywords = new List<string>();

        var locationVariations = new List<string>
        {
            $"{areaLower}",
            $"{areaLower} {cityLower}",
            $"{cityLower} {areaLower}"
        };

        var serviceVariations = GetServiceKeywordVariations(serviceName);

        foreach (var location in locationVariations)
        {
            foreach (var service in serviceVariations)
            {
                keywords.Add($"{service} {location}");

                // Service + Qualifier + Location
                foreach (var qualifier in qualifiers.Take(8))
                {
                    keywords.Add($"{qualifier} {service} {location}");
                }

                // Service + Problem + Location
                foreach (var problem in problems.Take(6))
                {
                    keywords.Add($"{service} {problem} {location}");
                }
            }
        }

        return keywords;
    }

    private int CalculateKeywordPriority(string keyword)
    {
        // Priority calculation based on:
        // 1. Contains city/area name (higher priority for local SEO)
        // 2. Contains transactional intent words
        // 3. Length (shorter = higher commercial intent)
        // 4. Contains problem/solution keywords

        int score = 0;

        if (keyword.Contains(City, StringComparison.OrdinalIgnoreCase)) score += 3;
        if (keyword.Contains("emergency") || keyword.Contains("same day")) score += 2;
        if (keyword.Contains("quote") || keyword.Contains("cost") || keyword.Contains("book")) score += 2;
        if (keyword.Contains("professional") || keyword.Contains("certified")) score += 1;
        if (keyword.Length < 40) score += 1;
        if (keyword.Contains("near me")) score += 2;

        return score;
    }
}