using AzureAdPoc.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

/// <summary>
///  Essas  classes sao opcao alternativa ao RoleAccessFilterAttribute.cs que é generico.
///  Assim cada filtro é independente, mas ha repeticao de codigo.
///  Ex: No AdminController, colocar o atributo [ServiceFilter(typeof(AdminFilterAttribute))]
/// </summary>
public class AdminFilterAttribute : Attribute, IActionFilter
{
    private readonly IAzureRoleService _azureRoleService;
    private readonly string[] _allowedRoles;

    public AdminFilterAttribute(IAzureRoleService azureRoleService)
    {
        _azureRoleService = azureRoleService;
        _allowedRoles = new[] { "AppAdmin" };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // NO OP
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        // Verificar se o usuário está autenticado
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult("The user is not logged in.")
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return;
        }

        // Verificar se o usuário tem permissão de admin
        if (!_azureRoleService.UserHasRole(user, _allowedRoles))
        {
            context.Result = new ObjectResult("The user does not have permissions to do this action.")
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
    }
}

// Filtro específico para Admin
public class DirectorFilterAttribute : Attribute, IActionFilter
{
    private readonly IAzureRoleService _azureRoleService;

    public DirectorFilterAttribute(IAzureRoleService azureRoleService)
    {
        _azureRoleService = azureRoleService;
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!CheckUserAccess(context, "AppAdmin","Director"))
            return;
    }

    private bool CheckUserAccess(ActionExecutingContext context, params string[] roles)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult("The user is not logged in.")
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return false;
        }

        if (!_azureRoleService.UserHasRole(user, roles))
        {
            context.Result = new ObjectResult("The user does not have permissions to do this action.")
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
            return false;
        }

        return true;
    }
}

// Filtro para Manager
public class ManagerFilterAttribute : Attribute, IActionFilter
{
    private readonly IAzureRoleService _azureRoleService;

    public ManagerFilterAttribute(IAzureRoleService azureRoleService)
    {
        _azureRoleService = azureRoleService;
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        CheckUserAccess(context, "Manager","Director", "AppAdmin");
    }

    private void CheckUserAccess(ActionExecutingContext context, params string[] roles)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult("The user is not logged in.")
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return;
        }

        if (!_azureRoleService.UserHasRole(user, roles))
        {
            context.Result = new ObjectResult("The user does not have permissions to do this action.")
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
    }
}

// Filtro para Employee (todos os usuários autenticados)
public class EmployeeFilterAttribute : Attribute, IActionFilter
{
    private readonly IAzureRoleService _azureRoleService;

    public EmployeeFilterAttribute(IAzureRoleService azureRoleService)
    {
        _azureRoleService = azureRoleService;
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        CheckUserAccess(context, "Employee", "Manager", "Director", "AppAdmin");
    }

    private void CheckUserAccess(ActionExecutingContext context, params string[] roles)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult("The user is not logged in.")
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return;
        }

        if (!_azureRoleService.UserHasRole(user, roles))
        {
            context.Result = new ObjectResult("The user does not have permissions to do this action.")
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
    }
}