using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Application.Features.Services;

namespace mvmclean.backend.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Promotion")] 
[Authorize(AuthenticationSchemes = "AdminCookie")] 
public class PromotionController : BaseAdminController
{
    public PromotionController(IMediator mediator) : base(mediator)
    {
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }


    
}