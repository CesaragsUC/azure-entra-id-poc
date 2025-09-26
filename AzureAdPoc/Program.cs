
// helpful: https://medium.com/@kagamimendoza/how-to-integrate-azure-active-directory-with-your-asp-net-core-project-24c1e9451243
// doc: https://learn.microsoft.com/en-us/entra/identity-platform/tutorial-web-app-dotnet-prepare-app?tabs=workforce-tenant
// also: https://learn.microsoft.com/en-us/entra/fundamentals/what-is-entra

using AzureAdPoc.Attributes;
using AzureAdPoc.Middleware;
using AzureAdPoc.Service;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// added this
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// added this
builder.Services.AddAuthorization();


// added this
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    });

// added this
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();


builder.Services.AddScoped<IAzureRoleService, AzureRoleService>();
builder.Services.AddScoped<AdminAccessFilterAttribute>();
builder.Services.AddScoped<ManagerAccessFilterAttribute>();
builder.Services.AddScoped<DirectorAccessFilterAttribute>();
builder.Services.AddScoped<EmployeeAccessFilterAttribute>();
builder.Services.AddScoped<RoleAccessFilterAttribute>(serviceProvider =>
{
    var azureRoleService = serviceProvider.GetRequiredService<IAzureRoleService>();
    return new RoleAccessFilterAttribute(azureRoleService, "Default");
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Error/HttpStatusCodeHandler/{0}");

    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();// added this
app.UseAuthorization();

//It has to be after UseAuthorization
app.UseMiddleware<AccessDeniedMiddleware>();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); // added this

app.Run();

