using MediatR;
using mvmclean.backend.Domain.Aggregates.Promotion;
using mvmclean.backend.Domain.Aggregates.Promotion.Enums;

namespace mvmclean.backend.Application.Features.Promotion.Queries;

public class GetPromotionByIdRequest : IRequest<GetPromotionByIdResponse>
{
    public Guid PromotionId { get; set; }
}

public class GetPromotionByIdResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public string Currency { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; }
    public List<string> ApplicablePostcodes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int RemainingUses => UsageLimit > 0 ? UsageLimit - UsedCount : -1;
    public bool IsExpired => DateTime.UtcNow > ValidTo;
}

public class GetPromotionByIdHandler : IRequestHandler<GetPromotionByIdRequest, GetPromotionByIdResponse>
{
    private readonly IPromotionRepository _promotionRepository;

    public GetPromotionByIdHandler(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<GetPromotionByIdResponse> Handle(GetPromotionByIdRequest request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdAsync(request.PromotionId);

        if (promotion == null)
            throw new KeyNotFoundException($"Promotion with ID {request.PromotionId} not found");

        return new GetPromotionByIdResponse
        {
            Id = promotion.Id,
            Code = promotion.Code,
            Description = promotion.Description,
            DiscountType = promotion.DiscountType,
            DiscountValue = promotion.DiscountValue,
            MinimumOrderAmount = promotion.MinimumOrderAmount.Amount,
            Currency = promotion.MinimumOrderAmount.Currency,
            ValidFrom = promotion.ValidFrom,
            ValidTo = promotion.ValidTo,
            UsageLimit = promotion.UsageLimit,
            UsedCount = promotion.UsedCount,
            IsActive = promotion.IsActive,
            ApplicablePostcodes = promotion.ApplicablePostcodes?.Select(p => p.Value).ToList() ?? new(),
            CreatedAt = promotion.CreatedAt,
            UpdatedAt = promotion.UpdatedAt
        };
    }
}
