using AzureAdPoc.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace AzureAdPoc.Attributes;

// Filtro genérico base
public class RoleAccessFilterAttribute : Attribute, IActionFilter
{
    private readonly IAzureRoleService _azureRoleService;
    protected readonly string[] AllowedRoles;

    public RoleAccessFilterAttribute(IAzureRoleService azureRoleService, params string[] allowedRoles)
    {
        _azureRoleService = azureRoleService;
        AllowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            // Redirecionar para login
            context.Result = new ChallengeResult();
            return;
        }

        if (!_azureRoleService.UserHasRole(user, AllowedRoles))
        {
            //context.Result = new ForbidResult();
            // OU definir o status code antes do redirect:
            context.HttpContext.Response.StatusCode = 403;
           context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
        }
    }
}

// Filtros específicos que herdam do genérico
public class AdminAccessFilterAttribute : RoleAccessFilterAttribute
{
    public AdminAccessFilterAttribute(IAzureRoleService azureRoleService)
        : base(azureRoleService, "AppAdmin")
    {
    }
}
public class DirectorAccessFilterAttribute : RoleAccessFilterAttribute
{
    public DirectorAccessFilterAttribute(IAzureRoleService azureRoleService)
        : base(azureRoleService, "Director", "AppAdmin")
    {
    }
}

public class ManagerAccessFilterAttribute : RoleAccessFilterAttribute
{
    public ManagerAccessFilterAttribute(IAzureRoleService azureRoleService)
        : base(azureRoleService, "Manager", "Director", "AppAdmin")
    {
    }
}

public class EmployeeAccessFilterAttribute : RoleAccessFilterAttribute
{
    public EmployeeAccessFilterAttribute(IAzureRoleService azureRoleService)
        : base(azureRoleService, "Employee", "Manager", "Director", "AppAdmin")
    {
    }
}