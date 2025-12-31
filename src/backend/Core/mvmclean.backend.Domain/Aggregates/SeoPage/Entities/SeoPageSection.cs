using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.Entities;

public class SeoPageSection : Entity
{
    public Guid SeoPageId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public int Order { get; private set; }
    
    public virtual SeoPage SeoPage { get; private set; }
    
    private SeoPageSection() { }
    
    public static SeoPageSection Create(string title, string content, int order = 0)
    {
        return new SeoPageSection
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            Order = order
        };
    }
}