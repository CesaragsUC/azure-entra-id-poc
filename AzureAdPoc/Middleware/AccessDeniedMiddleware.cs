namespace AzureAdPoc.Middleware;

public class AccessDeniedMiddleware
{
    private readonly RequestDelegate _next;

    public AccessDeniedMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.Redirect("/Error/AccessDenied");
        }

        // Tratar status codes específicos
        if (context.Response.StatusCode == 401)
        {
            context.Response.Redirect("/Error/Forbidden");
        }
        if (context.Response.StatusCode == 403)
        {
            context.Response.Redirect("/Error/AccessDenied");
        }
        if (context.Response.StatusCode == 404)
        {
            context.Response.Redirect("/Error/NotFound");
        }
    }
}
