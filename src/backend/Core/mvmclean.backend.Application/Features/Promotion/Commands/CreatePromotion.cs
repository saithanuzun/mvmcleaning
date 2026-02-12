using System.ComponentModel.DataAnnotations;
using MediatR;
using mvmclean.backend.Domain.Aggregates.Promotion;
using mvmclean.backend.Domain.Aggregates.Promotion.Enums;

namespace mvmclean.backend.Application.Features.Promotion.Commands;

public class CreatePromotionRequest : IRequest<CreatePromotionResponse>
{
    [Required]
    public string Code { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int DiscountType { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal DiscountValue { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    [Required]
    public DateTime ValidTo { get; set; }

    public int UsageLimit { get; set; }

    public decimal MinimumOrderAmount { get; set; }
}

public class CreatePromotionResponse
{
    public Guid PromotionId { get; set; }
    public string Message { get; set; }
}

public class CreatePromotionHandler : IRequestHandler<CreatePromotionRequest, CreatePromotionResponse>
{
    private readonly IPromotionRepository _promotionRepository;

    public CreatePromotionHandler(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<CreatePromotionResponse> Handle(CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        
        if (request.ValidFrom >= request.ValidTo)
            throw new ArgumentException("ValidFrom must be before ValidTo");

        var discountType = (DiscountType)request.DiscountType;

        // Convert unspecified DateTime to UTC for PostgreSQL compatibility
        var validFrom = request.ValidFrom.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.ValidFrom, DateTimeKind.Utc)
            : request.ValidFrom.ToUniversalTime();
        
        var validTo = request.ValidTo.Kind == DateTimeKind.Unspecified 
            ? DateTime.SpecifyKind(request.ValidTo, DateTimeKind.Utc)
            : request.ValidTo.ToUniversalTime();

        var promotion = Domain.Aggregates.Promotion.Promotion.Create(
            request.Code,
            request.Description,
            discountType,
            request.DiscountValue,
            validFrom,
            validTo,
            request.UsageLimit
        );

        await _promotionRepository.AddAsync(promotion);

        return new CreatePromotionResponse
        {
            PromotionId = promotion.Id,
            Message = "Promotion created successfully"
        };
    }
}
