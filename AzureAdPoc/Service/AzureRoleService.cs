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
            { "AppAdmin", "21cb50d1-dc09-47cb-9e83-82eee22242b0" },
            { "Manager", "1a19dcdb-86e3-4b54-a427-82f833cbca2e" },
            { "Director", "22f33b8e-2f00-48db-aae8-f27623a1b1df" },
            { "Employee", "dcf6845c-333e-4c4d-baa7-fad776917de2" }
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

        // Verificar claims de role primeiro
        if (user.IsInRole(roleName))
        {
            Console.WriteLine($"Found role claim for: {roleName}");
            return true;
        }

        // Verificar grupos
        if (_roleToGroupMapping.ContainsKey(roleName))
        {
            var groupId = _roleToGroupMapping[roleName];
            Console.WriteLine($"Looking for group ID: {groupId}");

            var hasGroup = user.Claims.Any(c => c.Type == "groups" && c.Value == groupId);
            Console.WriteLine($"Has group {groupId}: {hasGroup}");

            // Debug: listar todos os grupos
            var allGroups = user.Claims.Where(c => c.Type == "groups").Select(c => c.Value);
            Console.WriteLine($"All user groups: {string.Join(", ", allGroups)}");

            return hasGroup;
        }

        Console.WriteLine($"Role {roleName} not found in mapping");
        return false;
    }
}
