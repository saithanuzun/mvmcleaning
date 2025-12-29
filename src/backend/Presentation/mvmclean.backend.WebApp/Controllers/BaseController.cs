using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Controllers;

public abstract class BaseController : Controller
{
    protected bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;
    protected bool IsAdmin => User.IsInRole("Admin");
    protected bool IsContractor => User.IsInRole("Contractor");

    protected Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : (Guid?)null;
        }
    }

    protected string CurrentUserType => User.FindFirst("UserType")?.Value;
    protected string CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

    // Add success/error messages
    protected void Success(string message) => TempData["Success"] = message;
    protected void Error(string message) => TempData["Error"] = message;
    protected void Info(string message) => TempData["Info"] = message;
}
