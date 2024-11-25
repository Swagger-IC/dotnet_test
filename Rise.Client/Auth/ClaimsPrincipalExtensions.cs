using System.Security.Claims;

namespace Client.Auth;
public static class ClaimsPrincipalExtensions
{
    public static string GetFirstname(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));
            
        return principal.FindFirst("given_name")?.Value!;
    }

    public static string GetLastname(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));    

        return principal.FindFirst("family_name")?.Value!;
    }

    public static string GetFullname(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return GetFirstname(principal)+ " " + GetLastname(principal);
    }

    public static string GetPicture(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));
            
        return principal.FindFirst("picture")?.Value!;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return principal.FindFirst(ClaimTypes.Role)?.Value!;
    }

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return principal.FindFirst("name")?.Value!;
    }
}