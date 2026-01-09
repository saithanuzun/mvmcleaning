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
                    Areas = new[] { "City Centre", "Wigston", "Oadby", "Glen Parva", "Knighton", "Belgrave" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning" }
                }
            },
            {
                "London", new CityConfig
                {
                    City = "London",
                    Areas = new[] { "Westminster", "Camden", "Islington", "Hackney", "Tower Hamlets" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal", "Upholstery Cleaning" }
                }
            },
            {
                "Manchester", new CityConfig
                {
                    City = "Manchester",
                    Areas = new[] { "City Centre", "Fallowfield", "Ancoats", "Stockport" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal" }
                }
            },
            {
                "Birmingham", new CityConfig
                {
                    City = "Birmingham",
                    Areas = new[] { "City Centre", "Solihull", "Sutton Coldfield", "Edgbaston" },
                    Services = new[] { "Carpet Cleaning", "Sofa Cleaning", "Mattress Cleaning", "Stain Removal" }
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
