using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Contractor.Controllers;

[Area("Contractor")]
[Route("Contractor")]
public abstract class BaseContractorController : Controller
{
    protected readonly IMediator _mediator;

    protected BaseContractorController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public Guid? ContractorId
    {
        get
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var userClaims = identity.Claims;
                var id = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(id))
                    return new Guid(id);
            }

            return null;
        }
    }
}