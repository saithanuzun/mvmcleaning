using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.WebApp.Models;

namespace mvmclean.backend.WebApp.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    // GET: /Error
    [Route("/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Index()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        
        if (exceptionHandlerPathFeature?.Error != null)
        {
            var exception = exceptionHandlerPathFeature.Error;
            var path = exceptionHandlerPathFeature.Path;
            
            _logger.LogError(exception, "Unhandled exception occurred at {Path}", path);
            
        }

        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        };

        return View(model);
    }

    // GET: /Error/404
    [Route("/Error/404")]
    [AllowAnonymous]
    public IActionResult NotFoundError()
    {
        Response.StatusCode = 404;
        
        var originalPath = "unknown";
        if (HttpContext.Items.ContainsKey("originalPath"))
        {
            originalPath = HttpContext.Items["originalPath"] as string;
            _logger.LogWarning("404 error for path: {OriginalPath}", originalPath);
        }

        return View("NotFound");
    }

    // GET: /Error/403
    [Route("/Error/403")]
    [AllowAnonymous]
    public IActionResult AccessDeniedError()
    {
        Response.StatusCode = 403;
        
        var user = User.Identity?.Name ?? "Anonymous";
        var path = HttpContext.Request.Path;
        _logger.LogWarning("Access denied for user {User} at path {Path}", user, path);
        
        return View("AccessDenied");
    }

    // GET: /Error/500
    [Route("/Error/500")]
    [AllowAnonymous]
    public IActionResult InternalServerError()
    {
        Response.StatusCode = 500;
        return View("InternalServerError");
    }

    // GET: /Error/Status/{code}
    [Route("/Error/Status/{code:int}")]
    [AllowAnonymous]
    public IActionResult StatusCodeError(int code)
    {
        Response.StatusCode = code;
        
        var statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        if (statusCodeReExecuteFeature != null)
        {
            _logger.LogWarning(
                "Status code {StatusCode} for original path {OriginalPath}",
                code,
                statusCodeReExecuteFeature.OriginalPath
            );
        }

        return code switch
        {
            400 => View("BadRequest"),
            401 => View("Unauthorized"),
            403 => View("AccessDenied"),
            404 => View("NotFound"),
            500 => View("InternalServerError"),
            _ => View("Error")
        };
    }
}
