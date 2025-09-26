using AzureAdPoc.Attributes;
using AzureAdPoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureAdPoc.Controllers;

[Authorize]
[ServiceFilter(typeof(DirectorAccessFilterAttribute))]
public class DirectorController : Controller
{
    private readonly ILogger<DirectorController> _logger;

    public DirectorController(ILogger<DirectorController> logger)
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
