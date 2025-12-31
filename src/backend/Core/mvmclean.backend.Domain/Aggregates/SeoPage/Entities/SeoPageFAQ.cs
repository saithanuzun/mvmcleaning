using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.Entities;

public class SeoPageFAQ : Entity
{
    public Guid SeoPageId { get; private set; }
    public string Question { get; private set; }
    public string Answer { get; private set; }
    public int Order { get; private set; }
    
    public virtual SeoPage SeoPage { get; private set; }
    
    private SeoPageFAQ() { }
    
    public static SeoPageFAQ Create(string question, string answer, int order = 0)
    {
        return new SeoPageFAQ
        {
            Id = Guid.NewGuid(),
            Question = question,
            Answer = answer,
            Order = order
        };
    }
}