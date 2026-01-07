using MediatR;
using mvmclean.backend.Domain.Aggregates.Contractor;

namespace mvmclean.backend.Application.Features.Contractor.Commands;

public class LoginRequest: IRequest<LoginResponse>
{
    public string Usename { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string ContractorId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool Success { get; set; }
}

public class LoginHandler: IRequestHandler<LoginRequest,LoginResponse>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IMediator _mediator;

    public LoginHandler(IContractorRepository contractorRepository, IMediator mediator)
    {
        _contractorRepository = contractorRepository;
        _mediator = mediator;
    }


    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var encryptedPassword = Encryptor.PasswordEncryptor.Encrypt(request.Password);
        
        var username = request.Usename;

        var contractor = _contractorRepository.Get(i => i.Username == username && i.PasswordHash == encryptedPassword).FirstOrDefault();

        return new LoginResponse
        {           
            Success = contractor != null,
            ContractorId = contractor?.Id.ToString() ??  string.Empty,
            Username = username,
            Email = contractor.Email.ToString() ?? string.Empty,
        };
    }
}