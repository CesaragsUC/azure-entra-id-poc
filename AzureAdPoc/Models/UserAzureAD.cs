using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace AzureAdPoc.Models;

public class UserAzureAD
{
    public string user_name { get; set; }
    public string user_domain { get; set; }
    public string user_email { get; set; }

    /// <summary>
    /// Get the user name, user domain and email of the user from the authentication claims
    /// </summary>
    /// <param name="user">Auth Claims</param>
    /// <returns>Azure AD</returns>
    public static UserAzureAD GetUserOnAzureAd(ClaimsPrincipal user)
    {
        var preferredUsernameClaim = user.Claims.FirstOrDefault(c => c.Type.Equals("preferred_username"));
        if (preferredUsernameClaim != null)
        {
            return new UserAzureAD
            {
                user_name = user.Claims.FirstOrDefault(p => p.Type.Equals("name")).Value,
                user_email = preferredUsernameClaim.Value,
                user_domain = string.Format(@"cpiccr\{0}", preferredUsernameClaim.Value.Split('@')[0])
            };
        }
        return null; // Or throw an exception if preferred_username claim is required
    }

    public static void TransformGroupClaims(TokenValidatedContext context)
    {
        var groupMappings = new Dictionary<string, string>
        {
            { "21cb50d1-dc09-47cb-9e83-82eee22242b0", "AppAdmin" },
            { "1a19dcdb-86e3-4b54-a427-82f833cbca2e", "Manager" },
            { "22f33b8e-2f00-48db-aae8-f27623a1b1df", "Director" },
            { "dcf6845c-333e-4c4d-baa7-fad776917de2", "Employee" }
        };

        var identity = context.Principal.Identity as ClaimsIdentity;
        var groupClaims = context.Principal.Claims.Where(c => c.Type == "groups").ToList();

        foreach (var groupClaim in groupClaims)
        {
            if (groupMappings.ContainsKey(groupClaim.Value))
            {
                // Adiciona claim de role com nome amigável
                identity.AddClaim(new Claim(ClaimTypes.Role, groupMappings[groupClaim.Value]));
            }
        }
    }
}