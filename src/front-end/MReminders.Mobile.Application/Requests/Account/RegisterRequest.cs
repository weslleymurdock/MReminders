using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Domain.Entities;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Account;

public class RegisterRequest : IRequest<UserResponseBaseResponse>
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmationPassword { get; set; } = string.Empty;
    public string[] Roles { get; set; } = ["user"];
}
public class RegisterRequestHandler(IIdentityService identity, IMapper mapper, ILogger<RegisterRequestHandler> logger, IValidator<RegisterRequest> validator) : IRequestHandler<RegisterRequest, UserResponseBaseResponse>
{
    public async Task<UserResponseBaseResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return new UserResponseBaseResponse()
                {
                    Data = null!,
                    Message = string.Join(", ", validation.Errors),
                    StatusCode = int.TryParse(validation.Errors[0].ErrorCode, out int code) ? code : 422,
                    Success = validation.IsValid
                };
            }

            var appUser = mapper.Map<AppUser>(request);

            var response = await identity.Register(appUser, request.Password, request.Roles);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
