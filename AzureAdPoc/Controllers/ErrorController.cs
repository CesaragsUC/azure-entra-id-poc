using Microsoft.AspNetCore.Mvc;

namespace AzureAdPoc.Controllers;

public class ErrorController : Controller
{
    public IActionResult AccessDenied(string message = null, string requiredRoles = null, string currentUrl = null)
    {
        ViewBag.ErrorMessage = message ?? "Você não tem permissão para acessar esta página.";
        ViewBag.RequiredRoles = requiredRoles;
        ViewBag.CurrentUrl = currentUrl;
        ViewBag.ErrorTitle = "Acesso Negado";

        return View();
    }

    [Route("Error/HttpStatusCodeHandler/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        switch (statusCode)
        {
            case 403:
                return RedirectToAction("AccessDenied");
            case 401:
                return RedirectToAction("Forbidden");
            case 404:
                return RedirectToAction("NotFound");
            default:
                return View("Error");
        }
    }

    [Route("Error/NotFound")]
    public IActionResult NotFound()
    {
        return View();
    }


    [Route("Error/Forbidden")]
    public IActionResult Forbidden()
    {

        return View();

    }
}