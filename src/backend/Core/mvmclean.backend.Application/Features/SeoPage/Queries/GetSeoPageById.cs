using MediatR;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Application.Features.SeoPage.Queries;

public class GetSeoPageByIdRequest : IRequest<GetSeoPageByIdResponse>
{
    public Guid PageId { get; set; }
}

public class GetSeoPageByIdResponse
{
    public Guid Id { get; set; }
    public string Slug { get; set; }
    public string City { get; set; }
    public string? Area { get; set; }
    public string? ServiceType { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string H1Tag { get; set; }
    public string Introduction { get; set; }
    public List<SeoPageContentDto> ContentBlocks { get; set; } = new();
    public List<SeoPageFAQDto> FAQs { get; set; } = new();
    public List<SeoPageKeywordDto> Keywords { get; set; } = new();
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class SeoPageContentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int DisplayOrder { get; set; }
}

public class SeoPageFAQDto
{
    public Guid Id { get; set; }
    public string Question { get; set; }
    public string Answer { get; set; }
    public int DisplayOrder { get; set; }
}

public class SeoPageKeywordDto
{
    public Guid Id { get; set; }
    public string Keyword { get; set; }
    public int Priority { get; set; }
}

public class GetSeoPageByIdHandler : IRequestHandler<GetSeoPageByIdRequest, GetSeoPageByIdResponse>
{
    private readonly ISeoPageRepository _seoPageRepository;

    public GetSeoPageByIdHandler(ISeoPageRepository seoPageRepository)
    {
        _seoPageRepository = seoPageRepository;
    }

    public async Task<GetSeoPageByIdResponse> Handle(GetSeoPageByIdRequest request, CancellationToken cancellationToken)
    {
        var page = await _seoPageRepository.GetByIdAsync(request.PageId);

        if (page == null)
            throw new KeyNotFoundException($"SEO Page with ID {request.PageId} not found");

        return new GetSeoPageByIdResponse
        {
            Id = page.Id,
            Slug = page.Slug,
            City = page.City,
            Area = page.Area,
            ServiceType = page.ServiceType,
            MetaTitle = page.MetaTitle,
            MetaDescription = page.MetaDescription,
            H1Tag = page.H1Tag,
            Introduction = page.Introduction,
            ContentBlocks = page.ContentBlocks.Select(c => new SeoPageContentDto
            {
                Id = c.Id,
            }).OrderBy(c => c.DisplayOrder).ToList(),
            FAQs = page.FAQs.Select(f => new SeoPageFAQDto
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer,
            }).OrderBy(f => f.DisplayOrder).ToList(),
            Keywords = page.Keywords.Select(k => new SeoPageKeywordDto
            {
                Id = k.Id,
                Keyword = k.Keyword,
            }).OrderByDescending(k => k.Priority).ToList(),
            CreatedAt = page.CreatedAt,
        };
    }
}
