using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.Infrastructure.Persistence;
using mvmclean.backend.WebApp.Models;
using mvmclean.backend.WebApp.Models.Error;

namespace mvmclean.backend.WebApp.Controllers;

[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    // Handles unhandled exceptions
    [Route("/Error")]
    public IActionResult Index()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature?.Error != null)
        {
            _logger.LogError(
                exceptionFeature.Error,
                "Unhandled exception at path {Path}",
                exceptionFeature.Path
            );
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    // Handles HTTP status codes
    [Route("/Error/Status/{statusCode:int}")]
    public IActionResult Status(int statusCode)
    {
        Response.StatusCode = statusCode;
        ViewData["StatusCode"] = statusCode;

        var reExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        if (reExecuteFeature != null)
        {
            _logger.LogWarning(
                "HTTP {StatusCode} for path {OriginalPath}",
                statusCode,
                reExecuteFeature.OriginalPath
            );
        }

        return statusCode switch
        {
            400 => View("Errors/BadRequest"),
            401 => View("Errors/Unauthorized"),
            403 => View("Errors/AccessDenied"),
            404 => View("Errors/NotFound"),
            500 => View("Errors/InternalServerError"),
            _   => View("Errors/Error")
        };
    }
}