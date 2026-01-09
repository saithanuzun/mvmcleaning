using MediatR;
using mvmclean.backend.Domain.Aggregates.SeoPage;

namespace mvmclean.backend.Application.Features.SeoPage.Commands;

public class BulkCreateSeoPageRequest : IRequest<BulkCreateSeoPageResponse>
{
    public List<Domain.Aggregates.SeoPage.SeoPage> Pages { get; set; } = new();
}

public class BulkCreateSeoPageResponse
{
    public int CreatedCount { get; set; }
    public List<Guid> CreatedIds { get; set; } = new();
    public List<string> CreatedSlugs { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class BulkCreateSeoPageHandler : IRequestHandler<BulkCreateSeoPageRequest, BulkCreateSeoPageResponse>
{
    private readonly ISeoPageRepository _seoPageRepository;

    public BulkCreateSeoPageHandler(ISeoPageRepository seoPageRepository)
    {
        _seoPageRepository = seoPageRepository;
    }

    public async Task<BulkCreateSeoPageResponse> Handle(BulkCreateSeoPageRequest request, CancellationToken cancellationToken)
    {
        var response = new BulkCreateSeoPageResponse();

        foreach (var seoPage in request.Pages)
        {
            try
            {
                await _seoPageRepository.AddAsync(seoPage);
                response.CreatedIds.Add(seoPage.Id);
                response.CreatedSlugs.Add(seoPage.Slug);
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Failed to create {seoPage.Slug}: {ex.Message}");
            }
        }

        // Save all changes to database
        await _seoPageRepository.SaveChangesAsync();

        response.CreatedCount = response.CreatedIds.Count;
        return response;
    }
}
