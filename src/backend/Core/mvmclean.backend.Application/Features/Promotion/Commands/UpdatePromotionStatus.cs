using MediatR;
using mvmclean.backend.Domain.Aggregates.Promotion;

namespace mvmclean.backend.Application.Features.Promotion.Commands;

public class UpdatePromotionStatusRequest : IRequest<UpdatePromotionStatusResponse>
{
    public Guid PromotionId { get; set; }
    public bool IsActive { get; set; }
}

public class UpdatePromotionStatusResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class UpdatePromotionStatusHandler : IRequestHandler<UpdatePromotionStatusRequest, UpdatePromotionStatusResponse>
{
    private readonly IPromotionRepository _promotionRepository;

    public UpdatePromotionStatusHandler(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<UpdatePromotionStatusResponse> Handle(UpdatePromotionStatusRequest request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdAsync(request.PromotionId, noTracking: false);
        if (promotion == null)
            throw new KeyNotFoundException("Promotion not found");

        if (request.IsActive)
            promotion.Activate();
        else
            promotion.Deactivate();

        await _promotionRepository.SaveChangesAsync();

        return new UpdatePromotionStatusResponse
        {
            Success = true,
            Message = $"Promotion {(request.IsActive ? "activated" : "deactivated")} successfully"
        };
    }
}
