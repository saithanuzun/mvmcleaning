using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Infrastructure.Seeding;

/// <summary>
/// Helper to generate SEO page configurations for seeding
/// Creates 4 levels: CityOnly, CityService, CityArea, CityAreaService
/// </summary>
public static class SeoPageSeedData
{
    public static Dictionary<string, CityConfig> GetCitiesWithAreas()
    {
        return new Dictionary<string, CityConfig>
        {
            {
                "Leicester", new CityConfig
                {
                    City = "Leicester",
                    Areas = new[] { "City Centre", "Wigston", "Oadby", "Glen Parva", "Knighton", "Belgrave", "Syston", "Birstall", "Groby", "Kirby Muxloe", "Anstey", "Countesthorpe" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Coventry", new CityConfig
                {
                    City = "Coventry",
                    Areas = new[] { "City Centre", "Canley", "Earlsdon", "Keresley", "Holbrooks", "Whitley" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Loughborough", new CityConfig
                {
                    City = "Loughborough",
                    Areas = new[] { "Town Centre", "Quorn", "Shepshed", "Sileby" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Nuneaton", new CityConfig
                {
                    City = "Nuneaton",
                    Areas = new[] { "Town Centre", "Bedworth", "Arley", "Stockingford" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Rugby", new CityConfig
                {
                    City = "Rugby",
                    Areas = new[] { "Town Centre", "Hillmorton", "Lawford", "Brownsover" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Hinckley", new CityConfig
                {
                    City = "Hinckley",
                    Areas = new[] { "Town Centre", "Earl Shilton", "Barwell", "Desford", "Kirby Muxloe" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Melton Mowbray", new CityConfig
                {
                    City = "Melton Mowbray",
                    Areas = new[] { "Town Centre", "Asfordby", "Bottesford" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Market Harborough", new CityConfig
                {
                    City = "Market Harborough",
                    Areas = new[] { "Town Centre", "Lutterworth", "Broughton Astley" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Coalville", new CityConfig
                {
                    City = "Coalville",
                    Areas = new[] { "Town Centre", "Ibstock", "Heather" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Castle Donington", new CityConfig
                {
                    City = "Castle Donington",
                    Areas = new[] { "Town Centre", "Long Eaton", "Sawley" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Oakham", new CityConfig
                {
                    City = "Oakham",
                    Areas = new[] { "Town Centre", "Rutland Water", "Uppingham" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "West Bridgford", new CityConfig
                {
                    City = "West Bridgford",
                    Areas = new[] { "Town Centre", "Wilford", "Edwalton" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "Swadlincote", new CityConfig
                {
                    City = "Swadlincote",
                    Areas = new[] { "Town Centre", "Gresley", "Church Gresley" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            },
            {
                "London", new CityConfig
                {
                    City = "London",
                    Areas = new[] { "Westminster", "Camden", "Islington", "Hackney", "Tower Hamlets", "Bethnal Green", "Shoreditch", "Whitechapel", "Stepney", "Clerkenwell", "King's Cross", "Bloomsbury", "Soho", "Mayfair", "Kensington", "Chelsea", "Belgravia", "Knightsbridge" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning", "Jet Washing", "Pressure Washing" }
                }
            }
        };
    }

    /// <summary>
    /// Generate all SEO page objects for seeding
    /// Creates pages using single factory method and Add methods
    /// </summary>
    public static List<SeoPage> GenerateAllSeoPages()
    {
        var pages = new List<SeoPage>();
        var citiesConfig = GetCitiesWithAreas();
        
        foreach (var cityEntry in citiesConfig)
        {
            var cityConfig = cityEntry.Value;
            var servicesList = cityConfig.Services.ToList();
            var areasList = cityConfig.Areas.ToList();
            
            // 1. Create CityOnly page (e.g., /leicester)
            var cityPage = SeoPage.CreateSeoPage(cityConfig.City);
            pages.Add(cityPage);
            
            // 2. Create CityService pages (e.g., /leicester/carpet-cleaning)
            foreach (var service in servicesList)
            {
                var cityServicePage = SeoPage.CreateSeoPage(cityConfig.City);
                cityServicePage.AddService(service);
                pages.Add(cityServicePage);
            }
            
            // 3. Create CityArea pages (e.g., /leicester/wigston)
            foreach (var area in areasList)
            {
                var cityAreaPage = SeoPage.CreateSeoPage(cityConfig.City);
                cityAreaPage.AddArea(area);
                pages.Add(cityAreaPage);
            }
            
            // 4. Create CityAreaService pages (e.g., /leicester/wigston/carpet-cleaning)
            foreach (var area in areasList)
            {
                foreach (var service in servicesList)
                {
                    var fullPage = SeoPage.CreateSeoPage(cityConfig.City);
                    fullPage.AddArea(area);
                    fullPage.AddService(service);
                    pages.Add(fullPage);
                }
            }
        }
        
        return pages;
    }
}

public class CityConfig
{
    public string City { get; set; }
    public string[] Areas { get; set; } = Array.Empty<string>();
    public string[] Services { get; set; } = Array.Empty<string>();
}
