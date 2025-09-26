# Azure AD Authentication & Authorization POC

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![Azure AD](https://img.shields.io/badge/Authentication-Azure%20AD-blue)](https://azure.microsoft.com/en-us/services/active-directory/)

A **Proof of Concept (POC)** demonstrating role-based access control using **Azure Active Directory (Azure AD/Entra ID)** integration with **ASP.NET Core MVC**.

## 📋 Overview

This project implements a complete authentication and authorization system using Azure AD groups to control access to different areas of a web application. Users are authenticated through Azure AD and their group memberships determine which controllers and actions they can access.

![Azure AD Authentication Flow](https://img.shields.io/badge/Authentication-Azure%20AD-blue?style=flat-square&logo=microsoft)
![Authorization](https://img.shields.io/badge/Authorization-Role%20Based-green?style=flat-square)
![Framework](https://img.shields.io/badge/Framework-ASP.NET%20Core%208-purple?style=flat-square&logo=dotnet)

## ✨ Features

- **Azure AD Authentication** - Single Sign-On (SSO) with Microsoft accounts
- **Role-Based Access Control** - Group-based permissions from Azure AD
- **Custom Action Filters** - Attribute-based authorization for controllers
- **Dynamic Navigation** - Menu items show/hide based on user permissions
- **Custom Error Pages** - Beautiful 403 (Forbidden) and 404 (Not Found) pages
- **Real-time User Info** - Display current user's roles and permissions

## 🛠 Tech Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Authentication**: Microsoft Identity Web
- **Frontend**: Bootstrap 5, Font Awesome
- **Identity Provider**: Azure Active Directory (Entra ID)

## 🏗 Architecture

### Controllers Structure
```
Controllers/
├── AdminController.cs      # Admin-only access
├── ManagerController.cs    # Manager + Admin access
├── DirectorController.cs   # Director + Admin access
├── EmployeeController.cs   # All authenticated users
├── ErrorController.cs      # Error handling
└── HomeController.cs       # Public access
```

### Authorization Filters
```
Filters/
├── AdminConsoleAccessFilterAttribute.cs    # Admin-only filter
├── ManagerAccessFilterAttribute.cs         # Manager/Admin filter
├── DirectorAccessFilterAttribute.cs        # Director/Admin filter
└── EmployeeAccessFilterAttribute.cs        # All users filter
```

## 🚀 Setup Instructions

### 1. Azure AD Configuration

1. **Create Azure AD App Registration**:
   - Go to Azure Portal → Azure Active Directory → App registrations
   - Click "New registration"
   - Set redirect URI: `https://localhost:7xxx/signin-oidc`

2. **Create Security Groups**:
   ```
   - AppAdmin     (Full access)
   - Manager      (Management access)
   - Director     (Director access)
   - Employee     (Basic access)
   ```

3. **Assign Users to Groups**:
   - Azure AD → Groups → [Group Name] → Members → Add members

### 2. Application Configuration

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd azure-ad-auth-poc
   ```

2. **Update `appsettings.json`**:
   ```json
   {
     "AzureAd": {
       "Instance": "https://login.microsoftonline.com/",
       "TenantId": "your-tenant-id",
       "ClientId": "your-client-id",
       "CallbackPath": "/signin-oidc",
       "SignedOutCallbackPath": "/signout-callback-oidc"
     }
   }
   ```

3. **Update Group IDs in `AzureRoleService.cs`**:
   ```csharp
   private readonly Dictionary<string, string> _roleToGroupMapping = new()
   {
       { "AppAdmin", "your-appadmin-group-id" },
       { "Manager", "your-manager-group-id" },
       { "Director", "your-director-group-id" },
       { "Employee", "your-employee-group-id" }
   };
   ```

### 3. Configure Token Claims

1. **In Azure Portal**:
   - App registrations → Your app → Token configuration
   - Add groups claim → Security groups
   - Select ID and Access tokens

## 💻 Usage Examples

### Controller Protection

```csharp
// Admin-only controller
[Authorize]
[ServiceFilter(typeof(AdminConsoleAccessFilterAttribute))]
public class AdminController : Controller
{
    public IActionResult Index() => View();
}

// Manager/Admin controller
[Authorize]
[ServiceFilter(typeof(ManagerAccessFilterAttribute))]
public class ManagerController : Controller
{
    public IActionResult Index() => View();
}
```

### View-based Authorization

```html
@if (User.HasAzureRole("AppAdmin"))
{
    <a href="/Admin">Admin Panel</a>
}

@if (User.HasAzureRole("Manager", "AppAdmin"))
{
    <a href="/Manager">Manager Dashboard</a>
}
```

### Programmatic Role Checking

```csharp
public IActionResult SomeAction()
{
    if (User.HasAzureRole("AppAdmin"))
    {
        // Admin-specific logic
    }
    else if (User.HasAzureRole("Manager"))
    {
        // Manager-specific logic
    }
    
    return View();
}
```

## 👥 Role Hierarchy

| Role | Access Level | Can Access |
|------|-------------|------------|
| **AppAdmin** | Full Access | All controllers and actions |
| **Manager** | Management | Manager, Employee areas |
| **Director** | Executive | Director, Employee areas |
| **Employee** | Basic | Employee area only |

## 🔧 Key Components

### 1. Azure Role Service
Handles role checking logic and group-to-role mapping:

```csharp
public interface IAzureRoleService
{
    bool UserHasRole(ClaimsPrincipal user, params string[] roles);
    List<string> GetUserRoles(ClaimsPrincipal user);
}
```

### 2. Custom Action Filters
Attribute-based authorization for clean controller decoration:

```csharp
[ServiceFilter(typeof(AdminConsoleAccessFilterAttribute))]
public class AdminController : Controller { }
```

### 3. Extension Methods
Helper methods for easy role checking in views:

```csharp
public static bool HasAzureRole(this ClaimsPrincipal user, params string[] roles)
```

## ❌ Error Handling

The application includes custom error pages with modern designs:

- **403 Forbidden**: Beautiful access denied page with user information
- **404 Not Found**: Space-themed page not found with search functionality
- **500 Server Error**: Generic error handling

## 🔍 Development Notes

### Debugging Tips

1. **Check Group Claims**:
   ```csharp
   var groupClaims = User.Claims.Where(c => c.Type == "groups").ToList();
   ```

2. **Verify Group IDs**:
   - Azure Portal → Groups → [Group] → Overview → Object ID

3. **Test Role Assignment**:
   ```csharp
   var userRoles = _azureRoleService.GetUserRoles(User);
   ```

### Common Issues

1. **Groups not in token**: Configure token configuration in Azure AD
2. **Wrong Group IDs**: Copy exact Object IDs from Azure Portal
3. **Cache issues**: Clear browser cache and cookies
4. **Redirect loops**: Check middleware order in Program.cs

## 🔒 Security Considerations

- All sensitive areas require authentication
- Group membership is verified server-side
- Proper error handling prevents information disclosure
- HTTPS enforced in production
- Token validation through Microsoft Identity Web

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Microsoft Identity Web documentation
- Azure Active Directory team
- ASP.NET Core community

---

**Note**: This is a Proof of Concept for educational purposes. For production use, consider additional security measures, error handling, and monitoring.
