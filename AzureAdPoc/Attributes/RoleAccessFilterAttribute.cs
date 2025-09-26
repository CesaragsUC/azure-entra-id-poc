using AzureAdPoc.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AzureAdPoc.Attributes;

// Generic base filter
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
            // Redirect to login
            context.Result = new ChallengeResult();
            return;
        }

        if (!_azureRoleService.UserHasRole(user, AllowedRoles))
        {
            //context.Result = new ForbidResult();
            // OR set the status code before the redirect:
            context.HttpContext.Response.StatusCode = 403;
           context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
        }
    }
}

// Specific filters that inherit from the generic
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