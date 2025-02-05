using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Domain.Entities;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace MReminders.Mobile.Infrastructure.Services;

internal class IdentityService(MRemindersClient client, ITokenStorage storage, ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<UserResponse> GetProfile(string key) => (await client.GetUserAsync(key)).Data;

    public async Task<UserResponseBaseResponse> Register(AppUser user, string password, string[] roles)
    {
        try
        {
            logger.LogInformation($"{nameof(Register)} starts");
            var response = await client.RegisterAsync(new RegisterAccountRequest { UserName = user.UserName, Password = password, ConfirmationPassword = password, Email = user.Email, Name = user.Name, PhoneNumber = user.PhoneNumber, Roles = roles });
             
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(Register)} ends");
        }
    }


    public async Task SetBearer(string basic)
    {
        try
        {
            client.BasicToken = basic;
            var response = await client.LoginAsync();
            if (response.Success)
            {
                Token token = new()
                {
                    Id = new Guid().ToString(),
                    Expires = response.Data.ExpirationDate,
                    Kind = Domain.Enums.TokenKind.Bearer,
                    Value = response.Data.Token
                };
                await storage.SaveTokenAsync(token);
            }
            throw new UnauthorizedAccessException();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<BooleanBaseResponse> UpdateProfile(EditAccountProfileRequest request)
    {
        try
        {
            client.BearerToken = (await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer)).Value;
 
            return await client.UpdateProfileAsync(request);
        }
        catch (Exception e)
        {
            logger?.LogError(e, e.Message);
            throw;
        }
    }
}
