using System.Security.Claims;

namespace AzureAdPoc.Service;


public interface IAzureRoleService
{
    bool UserHasRole(ClaimsPrincipal user, params string[] roles);
    List<string> GetUserRoles(ClaimsPrincipal user);
    //bool UserIsAdmin(string username);
    //bool IsManager(string username);
    //bool IsDirector(string username);
    //bool IsEmployee(string username);
}

public class AzureRoleService : IAzureRoleService
{
    private readonly Dictionary<string, string> _roleToGroupMapping = new()
    {
        { "AppAdmin", "your-appadmin-group-id" },
        { "Manager", "your-manager-group-id" },
        { "Director", "your-director-group-id" },
        { "Employee", "your-employee-group-id" }
    };


    public bool UserHasRole(ClaimsPrincipal user, params string[] roles)
    {
        Console.WriteLine($"UserHasRole called with roles: {string.Join(", ", roles)}");
        Console.WriteLine($"User authenticated: {user.Identity.IsAuthenticated}");


        return roles.Any(role => HasSingleRole(user, role));
    }

    public List<string> GetUserRoles(ClaimsPrincipal user)
    {
        var userRoles = new List<string>();

        foreach (var roleMapping in _roleToGroupMapping)
        {
            if (HasSingleRole(user, roleMapping.Key))
            {
                userRoles.Add(roleMapping.Key);
            }
        }

        return userRoles;
    }

    private bool HasSingleRole(ClaimsPrincipal user, string roleName)
    {
        // Debug
        Console.WriteLine($"Checking role: {roleName}");

        // Verify groups
        if (_roleToGroupMapping.ContainsKey(roleName))
        {
            var groupId = _roleToGroupMapping[roleName];
            Console.WriteLine($"Looking for group ID: {groupId}");

            var hasGroup = user.Claims.Any(c => c.Type == "groups" && c.Value == groupId);
            Console.WriteLine($"Has group {groupId}: {hasGroup}");

            // Debug:  list all groups
            var allGroups = user.Claims.Where(c => c.Type == "groups").Select(c => c.Value);
            Console.WriteLine($"All user groups: {string.Join(", ", allGroups)}");

            return hasGroup;
        }

        Console.WriteLine($"Role {roleName} not found in mapping");
        return false;
    }
}
