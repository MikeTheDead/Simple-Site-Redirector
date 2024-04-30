using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using altsite.Models;
using altsite.MongoControllers;

namespace altsite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SiteMongoController siteController;

    public HomeController(ILogger<HomeController> logger,SiteMongoController _siteController)
    {
        _logger = logger;
        siteController = _siteController;
    }
    
    

    [HttpGet("sites")]
    public async Task<IActionResult> Index()
    {
        var sites = await siteController.GetCollection();
        return View(sites);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}