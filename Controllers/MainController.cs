using System.Diagnostics;
using System.Text.RegularExpressions;
using altsite.Attributes;
using Microsoft.AspNetCore.Mvc;
using altsite.Models;
using altsite.MongoControllers;
using MongoDB.Driver;

namespace altsite.Controllers;

public class MainController : Controller
{
    private readonly ILogger<MainController> _logger;
    private readonly SiteMongoController siteController;

    public MainController(ILogger<MainController> logger, SiteMongoController _siteController)
    {
        _logger = logger;
        siteController = _siteController;
    }
    [HttpGet("")]
    [HttpGet("home")]
    public IActionResult Home()
    {
        _logger.LogInformation("Redirecting to home.");
        return RedirectToAction("Index", "Home");
    }
    [HttpGet("/admin")]
    [Local]
    public async Task<IActionResult> Admin()
    {
        var sites = await siteController.GetCollection();
        return View(sites);
    }
    [HttpPost]
    [Local]
    public async Task<ActionResult> AddOriginalSite(string newSite)
    {
        Console.WriteLine(newSite);
        Site? site = await siteController.Get(newSite);
        if (site == null)
        {
            site = new Site
            {
                OriginalSiteURL = newSite,
                RedirectableSites = new List<string>(),
                LastRedirection = " "
            };

            await siteController.Set(site);
        }
        
        return Content(newSite);
    }
    [HttpPost]
    [Local]
    public async Task<ActionResult> AddSite(string newSite, string originalSite)
    {
        Console.WriteLine(newSite);
        Console.WriteLine(originalSite);
        Site? site = await siteController.Get(originalSite);
        if (site != null)
        {
            site.RedirectableSites.Add(newSite);

            await siteController.Update(site);
        }
        
        return Content(newSite);
    }
    [HttpPost]
    [Local]
    public async Task<ActionResult> RemoveOriginalSite(string oldSite)
    {
        Console.WriteLine(oldSite);
        Site? site = await siteController.Get(oldSite);
        if (site != null)
        {
            await siteController.Remove(site);
        }
        
        return Content(oldSite);
    }
    [HttpPost]
    [Local]
    public async Task<ActionResult> RemoveSite(string removableSite, string originalSite)
    {
        Console.WriteLine(removableSite);
        Console.WriteLine(originalSite);
        Site? site = await siteController.Get(originalSite);
        if (site != null)
        {
            site.RedirectableSites.Remove(removableSite);

            await siteController.Update(site);
        }
        return Content(removableSite);
    }

    [HttpGet("{*url}")]
    public async Task<IActionResult> Index(string url, [FromQuery]string query = null)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            url = query;
        }
    
        if (string.IsNullOrWhiteSpace(url))
        {
            return RedirectToAction("Home", "Home");
        }
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "http://" + url;
        }
    
        Uri uri;
        if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            _logger.LogError("Invalid URL provided.");
            return RedirectToAction("Home", "Home");
        }

        string hostname = uri.Host;
        Site? site = await siteController.Get(hostname);
        if (site == null)
        {
            _logger.LogInformation("No matching site found for redirection.");
            return Redirect(url); 
        }

        string pathOnly = Regex.Replace(url, @"^https?:\/\/[^\/]+\/", "");
        string? altSite = site.RedirectableSites.FirstOrDefault(s => s != site.LastRedirection);
        if (!string.IsNullOrEmpty(altSite))
        {
            site.LastRedirection = altSite;
            await siteController.Update(site);
            string redir = $"https://{altSite}/{pathOnly}";
            return Redirect(redir);
        }
        else
        {
            return Redirect(url); 
        }
    }

    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}