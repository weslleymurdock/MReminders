using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Account;

public class GetProfileRequest : IRequest<UserResponse> 
{ 
}

public class GetProfileRequestHandler(IIdentityService identity, IProtectedStorage<string> stringStorage, ILogger<GetProfileRequestHandler> logger) : IRequestHandler<GetProfileRequest, UserResponse>
{
    public async Task<UserResponse> Handle(GetProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await stringStorage.GetAsync(stringStorage.UserId);
            var response = await identity.GetProfile(userId);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
