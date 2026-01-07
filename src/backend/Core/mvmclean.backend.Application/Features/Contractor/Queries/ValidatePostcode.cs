using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Queries;

public class ValidatePostcodeRequest : IRequest<ValidatePostcodeResponse>
{
    public string Postcode { get; set; }
}

public class ValidatePostcodeResponse
{
    public bool IsValid { get; set; }
    public bool IsCovered { get; set; }
    public string Postcode { get; set; }
}

public class ValidatePostcodeHandler : IRequestHandler<ValidatePostcodeRequest, ValidatePostcodeResponse>
{
    private readonly IContractorRepository _contractorRepository;

    public ValidatePostcodeHandler(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }

    public async Task<ValidatePostcodeResponse> Handle(ValidatePostcodeRequest request, CancellationToken cancellationToken)
    {
        // Basic validation - you can add regex validation here if needed
        bool isValid = !string.IsNullOrWhiteSpace(request.Postcode);

        // Check if any active contractor covers this postcode
        var contractors = await _contractorRepository.GetAll(false);
        bool isCovered = contractors.Any(c =>
            c.IsActive &&
            c.CoverageAreas != null &&
            c.CoverageAreas.Any(ca => ca.Postcode.Value.Equals(request.Postcode, StringComparison.OrdinalIgnoreCase)));

        return new ValidatePostcodeResponse
        {
            IsValid = isValid,
            IsCovered = isCovered,
            Postcode = request.Postcode
        };
    }
}
