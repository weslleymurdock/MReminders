using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MReminders.API.Infrastructure.Interfaces;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MReminders.API.Infrastructure.Handlers;

public class BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IIdentityService identityService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues authorizationHeader))
            return await Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
 
        try
        {
            var header = AuthenticationHeaderValue.Parse(authorizationHeader!);
            var credentials = Encoding.UTF8
                .GetString(Convert.FromBase64String(header.Parameter!.Replace("Basic ", "") ?? string.Empty))
                .Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            var success = await identityService.SigninUser(username, password);

            if (success)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return await Task.FromResult(AuthenticateResult.Fail("Invalid password or username"));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }
    }
}