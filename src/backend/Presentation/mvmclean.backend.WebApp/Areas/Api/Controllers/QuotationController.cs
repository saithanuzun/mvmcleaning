using Microsoft.AspNetCore.Mvc;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

public class QuotationController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}