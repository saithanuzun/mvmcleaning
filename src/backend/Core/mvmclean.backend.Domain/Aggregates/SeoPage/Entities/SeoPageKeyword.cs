using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.Entities;

public class SeoPageKeyword : Entity
{
    public Guid SeoPageId { get; private set; }
    public string Keyword { get; private set; }
    public string ServiceType { get; private set; }
    public int SearchVolume { get; private set; }
    public decimal Competition { get; private set; }
    
    public virtual SeoPage SeoPage { get; private set; }
    
    private SeoPageKeyword() { }
    
    public static SeoPageKeyword Create(string keyword, string serviceType, 
        int searchVolume = 0, decimal competition = 0)
    {
        return new SeoPageKeyword
        {
            Id = Guid.NewGuid(),
            Keyword = keyword,
            ServiceType = serviceType,
            SearchVolume = searchVolume,
            Competition = competition
        };
    }
}
