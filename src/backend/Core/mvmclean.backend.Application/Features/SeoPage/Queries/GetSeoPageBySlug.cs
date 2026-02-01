using MediatR;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Application.Features.SeoPage.Queries;

public class GetSeoPageBySlugRequest : IRequest<GetSeoPageBySlugResponse>
{
    public required string Slug { get; set; }
}

public class GetSeoPageBySlugResponse
{
    public SeoPageDetailDto? Page { get; set; }
}

public class SeoPageDetailDto
{
    public Guid Id { get; set; }
    public required string Slug { get; set; }
    public required string Level { get; set; }
    public required string City { get; set; }
    public string? Area { get; set; }
    public string? Service { get; set; }
    public required string MetaTitle { get; set; }
    public required string MetaDescription { get; set; }
    public required string H1Tag { get; set; }
    public required string Introduction { get; set; }
    public string? AreasServed { get; set; }
    public string? ServicesOffered { get; set; }
    public List<SeoPageKeywordDto> Keywords { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class GetSeoPageBySlugHandler : IRequestHandler<GetSeoPageBySlugRequest, GetSeoPageBySlugResponse>
{
    private readonly ISeoPageRepository _seoPageRepository;

    public GetSeoPageBySlugHandler(ISeoPageRepository seoPageRepository)
    {
        _seoPageRepository = seoPageRepository;
    }

    public async Task<GetSeoPageBySlugResponse> Handle(GetSeoPageBySlugRequest request,
        CancellationToken cancellationToken)
    {
        // Fetch the page by slug
        var page = await _seoPageRepository.FirstOrDefaultAsync(i=>i.Slug == request.Slug,false,k=>k.Keywords);
     
        if (page == null)
            return new GetSeoPageBySlugResponse { Page = null };

        var pageDto = new SeoPageDetailDto
        {
            Id = page.Id,
            Slug = page.Slug,
            Level = page.Level.ToString(),
            City = page.City,
            Area = page.GetFirstAreaName(),
            Service = page.GetFirstServiceName(),
            MetaTitle = page.MetaTitle,
            MetaDescription = page.MetaDescription,
            H1Tag = page.H1Tag,
            Introduction = page.Introduction,
            AreasServed = page.GetAreasAsString(),
            ServicesOffered = page.GetServicesAsString(),
            CreatedAt = page.CreatedAt,
            Keywords = page.Keywords.Select(k => new SeoPageKeywordDto
            {
                Keyword = k.Keyword,
                Category = k.Category
            }).ToList()
        };

        return new GetSeoPageBySlugResponse { Page = pageDto };
    }


}