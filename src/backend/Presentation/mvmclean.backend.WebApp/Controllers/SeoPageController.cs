using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvmclean.backend.Application.Features.SeoPage.Queries;

namespace mvmclean.backend.WebApp.Controllers;

public class SeoPageController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<SeoPageController> _logger;

    public SeoPageController(IMediator mediator, ILogger<SeoPageController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// - /leicester (city level)
    /// - /leicester/wigston (city + area)
    /// - /leicester/wigston/carpet-cleaning (city + area + service)
    /// </summary>
    [HttpGet("/{**slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        // Don't match file extensions
        if (slug.Contains("."))
        {
            return RedirectToAction("Index","Home");
        }          
        
        try
        {
            
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            // Get the SEO page by slug
            var response = await _mediator.Send(new GetSeoPageBySlugRequest { Slug = slug });
            
            if (response?.Page == null)
            {
                return NotFound();
            }

            
            // Set ViewData for SEO meta tags
            ViewData["Title"] = response.Page.MetaTitle;
            ViewData["MetaDescription"] = response.Page.MetaDescription;
            ViewData["MetaKeywords"] = string.Join(", ", response.Page.Keywords.Select(k => k.Keyword));
            ViewData["H1Title"] = response.Page.H1Tag;
            ViewData["CanonicalUrl"] = $"https://mvmcleaning.com/{response.Page.Slug}";

            // Additional data for display
            ViewData["AreasServed"] = response.Page.AreasServed;
            ViewData["ServicesOffered"] = response.Page.ServicesOffered;

            return View("SeoPage", response.Page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"SeoPageController: Exception while fetching slug: '{slug}'");
            return NotFound();
        }
    }

    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        var response = await _mediator.Send(new GetAllSeoPagesRequest());
        return Ok(new 
        { 
            totalPages = response?.Pages?.Count ?? 0,
            pages = response?.Pages?.Select(p => new { p.Slug, p.City, p.Area, p.ServiceType }).ToList()
        });
    }

}
