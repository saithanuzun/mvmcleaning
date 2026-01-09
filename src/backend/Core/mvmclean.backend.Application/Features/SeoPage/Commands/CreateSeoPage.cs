using MediatR;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Application.Features.SeoPage.Commands;

public class CreateSeoPageRequest : IRequest<CreateSeoPageResponse>
{
    public string City { get; set; }
    public string Area { get; set; }
    public string? ServiceType { get; set; }
}

public class CreateSeoPageResponse
{
    public Guid SeoPageId { get; set; }
    public string Slug { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string H1Tag { get; set; }
}

public class CreateSeoPageHandler : IRequestHandler<CreateSeoPageRequest, CreateSeoPageResponse>
{
    private readonly ISeoPageRepository _seoPageRepository;

    public CreateSeoPageHandler(ISeoPageRepository seoPageRepository)
    {
        _seoPageRepository = seoPageRepository;
    }

    public async Task<CreateSeoPageResponse> Handle(CreateSeoPageRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Use BulkCreateSeoPageRequest with specific factory methods (CreateCityPage, CreateAreaPage, CreateCityAreaPage, CreateCityAreaServicePage) instead");
    }
}
