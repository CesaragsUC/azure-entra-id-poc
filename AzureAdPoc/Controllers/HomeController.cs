using AzureAdPoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AzureAdPoc.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {

        var groupMappings = new Dictionary<string, string>
        {
            { "21cb50d1-dc09-47cb-9e83-82eee22242b0", "AppAdmin" },
            { "1a19dcdb-86e3-4b54-a427-82f833cbca2e", "Manager" },
            { "22f33b8e-2f00-48db-aae8-f27623a1b1df", "Director" },
            { "dcf6845c-333e-4c4d-baa7-fad776917de2", "Employee" }
        };

        var groupClaims = User.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();
        var roleClaims = groupClaims.Select(g =>
            groupMappings.ContainsKey(g) ? groupMappings[g] : g).ToList();

        ViewBag.UserGroupNames = groupClaims.Select(g =>
            groupMappings.ContainsKey(g) ? groupMappings[g] : g).ToList();


        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
