using MediatR;
using mvmclean.backend.Domain.Aggregates.Promotion;
using mvmclean.backend.Domain.Aggregates.Promotion.Enums;

namespace mvmclean.backend.Application.Features.Promotion;

public class GetAllPromotionsRequest : IRequest<GetAllPromotionsResponse>
{
}

public class GetAllPromotionsResponse
{
    public List<PromotionDto> Promotions { get; set; } = new();
}

public class PromotionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; }
    public int ApplicablePostcodesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusBadgeClass => IsActive && DateTime.UtcNow >= ValidFrom && DateTime.UtcNow <= ValidTo 
        ? "success" 
        : IsActive ? "warning" : "danger";
}

public class GetAllPromotionsHandler : IRequestHandler<GetAllPromotionsRequest, GetAllPromotionsResponse>
{
    private readonly IPromotionRepository _promotionRepository;

    public GetAllPromotionsHandler(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<GetAllPromotionsResponse> Handle(GetAllPromotionsRequest request, CancellationToken cancellationToken)
    {
        var promotions = await _promotionRepository.GetAll(false);

        var promotionDtos = promotions
            .Select(p => new PromotionDto
            {
                Id = p.Id,
                Code = p.Code,
                Description = p.Description,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                MinimumOrderAmount = p.MinimumOrderAmount.Amount,
                ValidFrom = p.ValidFrom,
                ValidTo = p.ValidTo,
                UsageLimit = p.UsageLimit,
                UsedCount = p.UsedCount,
                IsActive = p.IsActive,
                ApplicablePostcodesCount = p.ApplicablePostcodes?.Count ?? 0,
                CreatedAt = p.CreatedAt
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return new GetAllPromotionsResponse { Promotions = promotionDtos };
    }
}
