using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Account;

public class EditProfileRequest : IRequest<BooleanBaseResponse> 
{

    public string Name = string.Empty;

    public string Email = string.Empty;

    public string Phone = string.Empty;

    public string OldPassword = string.Empty;

    public string NewPassword = string.Empty;

    public string ConfirmationNewPassword = string.Empty;

}

public class EditProfileRequestHandler(IIdentityService identity, IValidator<EditProfileRequest> validator, IProtectedStorage<string> stringStorage, ILogger<EditProfileRequestHandler> logger) : IRequestHandler<EditProfileRequest, BooleanBaseResponse>
{
    public async Task<BooleanBaseResponse> Handle(EditProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return new BooleanBaseResponse()
                {
                    Data = validation.IsValid,
                    Message = string.Join("\n ", validation.Errors),
                    StatusCode = int.TryParse(validation.Errors.FirstOrDefault()!.ErrorCode, out var code) ? code : 422,
                    Success = validation.IsValid
                };
            }
            var userId = await stringStorage.GetAsync(stringStorage.UserId);
            var _request = new EditAccountProfileRequest() { Email = request.Email, Id = userId, Name = request.Name, OldPassword = request.OldPassword, Password = request.NewPassword, PhoneNumber = request.Phone };

            var response = await identity.UpdateProfile(_request);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
