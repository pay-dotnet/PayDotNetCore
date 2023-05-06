using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PayDotNet.Core.Abstraction;

namespace PayDotNet.Core.Services;

/// <summary>
/// Default implementation of the context
/// </summary>
public class PayDefaultCustomerEmailResolverService : IPayCustomerEmailResolverService
{
    protected readonly IHttpContextAccessor HttpContextAccessor;

    public PayDefaultCustomerEmailResolverService(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public virtual string ResolveCustomerEmail()
    {
        ClaimsPrincipal user = HttpContextAccessor.HttpContext.User;
        if (user is null)
        {
            throw new PayDotNetException("HttpContext.User doesn't have a value. Please make sure you have enabled the authentication. See the property HelpLink for more information.")
            {
                HelpLink = "https://learn.microsoft.com/en-us/aspnet/core/security/authentication/"
            };
        }

        if (!user.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            throw new PayDotNetException($"The user doesn't have the ClaimType 'Email' that can be found in Type '{ClaimTypes.Email}'. Please make sure the user has this claim. See the property HelpLink for more information.")
            {
                HelpLink = "https://learn.microsoft.com/en-us/aspnet/core/security/authentication/claims",
            };
        }

        return user.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
    }
}