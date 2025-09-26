
// helpful: https://medium.com/@kagamimendoza/how-to-integrate-azure-active-directory-with-your-asp-net-core-project-24c1e9451243
// doc: https://learn.microsoft.com/en-us/entra/identity-platform/tutorial-web-app-dotnet-prepare-app?tabs=workforce-tenant

using AzureAdPoc.Attributes;
using AzureAdPoc.Middleware;
using AzureAdPoc.Service;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// added this
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("groups", "21cb50d1-dc09-47cb-9e83-82eee22242b0"));

    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireClaim("groups", "21cb50d1-dc09-47cb-9e83-82eee22242b0", "1a19dcdb-86e3-4b54-a427-82f833cbca2e"));

    options.AddPolicy("DirectorOnly", policy =>
        policy.RequireClaim("groups", "21cb50d1-dc09-47cb-9e83-82eee22242b0", "22f33b8e-2f00-48db-aae8-f27623a1b1df"));

    options.AddPolicy("EmployeeAccess", policy =>
        policy.RequireClaim("groups", "dcf6845c-333e-4c4d-baa7-fad776917de2", "22f33b8e-2f00-48db-aae8-f27623a1b1df", "1a19dcdb-86e3-4b54-a427-82f833cbca2e", "21cb50d1-dc09-47cb-9e83-82eee22242b0"));
});


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

//tem q ser depois do UseAuthorization
app.UseMiddleware<AccessDeniedMiddleware>();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); // added this

app.Run();

