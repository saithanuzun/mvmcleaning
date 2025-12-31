using mvmclean.backend.Domain.Core.BaseClasses;

namespace mvmclean.backend.Domain.Aggregates.SeoPage.Entities;

public class SeoPageContent : Entity
{
    public string Heading { get; private set; }
    public string Paragraph { get; private set; }

    private SeoPageContent() { }

    public static SeoPageContent Create(string heading, string paragraph)
    {
        return new SeoPageContent
        {
            Heading = heading,
            Paragraph = paragraph
        };
    }
}
