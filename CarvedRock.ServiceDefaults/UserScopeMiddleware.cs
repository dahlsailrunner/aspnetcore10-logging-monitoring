using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CarvedRock.ServiceDefaults;

public class UserScopeMiddleware(RequestDelegate next, ILogger<UserScopeMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var user = context.User;
            var claimsToInclude = new List<string> {
                    "sub", "amr", "scope", "name", "email", ClaimTypes.Role
            };

            var claimDictionary = new Dictionary<string, object>();
            foreach (var claimType in claimsToInclude)
            {
                claimDictionary.Add($"claim:{claimType}", string.Join(", ",
                    user.Claims
                    .Where(c => c.Type == claimType)
                    .Select(c => c.Value)));
            }

            using var scope = logger.BeginScope(claimDictionary);
            await next(context);
        }
        else
        {
            await next(context);
        }
    }
}