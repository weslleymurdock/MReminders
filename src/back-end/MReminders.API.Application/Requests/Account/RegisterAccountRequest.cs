using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Domain.Identity;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Account;

public class RegisterAccountRequest : IRequest<BaseResponse<UserResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ConfirmationPassword { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = ["user"];
}

public class AccountRegisterRequestHandler(IIdentityService service, ILogger<AccountRegisterRequestHandler> logger, IValidator<RegisterAccountRequest> validator, IMapper mapper) : IRequestHandler<RegisterAccountRequest, BaseResponse<UserResponse>>
{
    public async Task<BaseResponse<UserResponse>> Handle(RegisterAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"{nameof(AccountRegisterRequestHandler)} started");
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                return new() { Data = null!, Message = string.Join("\n ", result.Errors), Success = false, StatusCode = 422 };
            }

            var user = mapper.Map<AppUser>(request);
            var (isSucceed, userId) = await service.CreateUserAsync(request.UserName, request.Password, request.Email, request.Name, request.PhoneNumber, request.Roles);
            if (isSucceed)
            {
                user.Id = userId;
                user.Roles = request.Roles.Select(x => new AppRole() { Name = x }).ToList();
                return new BaseResponse<UserResponse>()
                {
                    Data = mapper.Map<UserResponse>(user),
                    Success = true,
                    Message = "Created User",
                    StatusCode = 201
                };
            }
            return new BaseResponse<UserResponse>()
            {
                Data = default!,
                Success = false,
                Message = "User not created",
                StatusCode = 409
            };

        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new BaseResponse<UserResponse>()
            {
                Data = null!,
                Message = $"There was a error: {e.Message}",
                Success = false,
                StatusCode = 400
            };
        }
        finally
        {
            logger.LogInformation($"{nameof(AccountRegisterRequestHandler)} finishes");
        }
    }
}

