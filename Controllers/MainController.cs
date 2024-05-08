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
public async Task<IActionResult> index(string url, [FromQuery]string query = null)
{
    try
    {
        //log the provided url
        _logger.LogInformation($"requested url: {url}");

        //if query parameter is provided, use it as the url
        if (!string.IsNullOrWhiteSpace(query))
        {
            url = query;
        }

        //if url is empty or null, redirect to home
        if (string.IsNullOrWhiteSpace(url))
        {
            return RedirectToAction("Home", "Home");
        }

        //prepend "http://" if url doesn't start with it
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "http://" + url;
        }

        //validate the url
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || uri == null)
        {
            //log error for invalid url
            _logger.LogError($"invalid url provided: {url}");
            return RedirectToAction("Home", "Home");
        }

        //extract hostname and normalize it
        string hostname = NormalizeDomain(uri.Host);
        _logger.LogInformation($"normalized hostname: {hostname}");

        //retrieve site based on normalized hostname
        Site? site = await siteController.Get(hostname);
        if (site == null)
        {
            _logger.LogInformation($"no matching site found for redirection: {hostname}");
            return Redirect(url);
        }

        _logger.LogInformation(site.OriginalSiteURL + ' ' + site.LastRedirection);
        //extract path from url
        string pathOnly = uri.PathAndQuery;

        //find alternative site for redirection
        string? altSite = site.RedirectableSites.FirstOrDefault(s => s != site.LastRedirection) ?? site.RedirectableSites.FirstOrDefault();

        if (!string.IsNullOrEmpty(altSite))
        {
            //update lastredirection property of site
            site.LastRedirection = altSite;
            await siteController.Update(site);

            //construct redirection url with alternative site and original path
            string redir = $"https://{altSite}{pathOnly}";
            _logger.LogInformation(redir);
            //redirect to the new url
            return Redirect(redir);
        }
        else
        {
            _logger.LogInformation(altSite);
            //redirect to the original url
            return Redirect(url);
        }
    }
    catch (Exception ex)
    {
        //log any unhandled exceptions
        _logger.LogError($"an error occurred: {ex.Message}");
        return RedirectToAction("Home", "Home");
    }
}

private string NormalizeDomain(string host)
{
    var parts = host.Split('.');
    if (parts.Length > 2)
    {
        // remove subdomain
        return string.Join(".", parts.Skip(1));
    }
    return host;
}



    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}