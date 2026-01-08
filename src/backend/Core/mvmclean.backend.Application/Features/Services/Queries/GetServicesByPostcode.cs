using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;
using mvmclean.backend.Domain.Aggregates.Service;
using mvmclean.backend.Domain.SharedKernel.ValueObjects;

namespace mvmclean.backend.Application.Features.Services.Queries;

public class GetServicesByPostcodeRequest : IRequest<List<GetServicesByPostcodeResponse>>
{
    public string Postcode { get; set; }
}

public class GetServicesByPostcodeResponse
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string Description { get; set; }
    public decimal AdjustedPrice { get; set; }
    public string Category { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}

public class GetServicesByPostcodeHandler : IRequestHandler<GetServicesByPostcodeRequest, List<GetServicesByPostcodeResponse>>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IContractorRepository _contractorRepository;

    public GetServicesByPostcodeHandler(
        IServiceRepository serviceRepository,
        IContractorRepository contractorRepository)
    {
        _serviceRepository = serviceRepository;
        _contractorRepository = contractorRepository;
    }

    public async Task<List<GetServicesByPostcodeResponse>> Handle(GetServicesByPostcodeRequest request,
        CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrEmpty(request.Postcode))
            return new List<GetServicesByPostcodeResponse>();

        try
        {
            // Create postcode value object for price calculations
            var postcode = Postcode.Create(request.Postcode);

            // Get all services
            var allServices = await _serviceRepository.GetAll(noTracking: true);

            // Get contractors covering this postcode - EXECUTE THE QUERY FIRST
            var contractorsInArea = (_contractorRepository.Get(
                    predicate: null,
                    false,
                    i => i.CoverageAreas))
                .ToList(); // Execute query here

            // Now filter in memory
            var activeContractors = contractorsInArea
                .Where(c => c.CoversPostcode(postcode))
                .ToList();

            if (!activeContractors.Any())
                return new List<GetServicesByPostcodeResponse>();

            var servicesInArea = new List<GetServicesByPostcodeResponse>();

            foreach (var service in allServices)
            {
                var contractorOfferingService = activeContractors
                    .FirstOrDefault(c => c.Services.Any(s => s.ServiceId == service.Id));

                if (contractorOfferingService != null)
                {
                    // Get adjusted price for this postcode using domain method
                    var adjustedPrice = service.GetAdjustedPriceForPostcode(postcode);

                    servicesInArea.Add(new GetServicesByPostcodeResponse
                    {
                        ServiceId = service.Id,
                        ServiceName = service.Name,
                        Description = service.Description,
                        AdjustedPrice = adjustedPrice.Amount,
                        Category = service.Category?.Name ?? "Uncategorized",
                        EstimatedDurationMinutes = (int)service.EstimatedDuration.TotalMinutes
                    });
                }
            }

            // Remove duplicates and return
            return servicesInArea
                .GroupBy(s => s.ServiceId)
                .Select(g => g.First())
                .OrderBy(s => s.ServiceName)
                .ToList();
        }
        catch (Exception)
        {
            return new List<GetServicesByPostcodeResponse>();
        }
    }
}