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
            { "your-appadmin-group-id" , "AppAdmin" },
            { "your-manager-group-id", "Manager" },
            { "your-director-group-id", "Director" },
            { "your-employee-group-id", "Employee" }
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
