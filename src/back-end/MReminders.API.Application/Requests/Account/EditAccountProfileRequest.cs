using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Domain.Identity;
using MReminders.API.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace MReminders.API.Application.Requests.Account;

public class EditAccountProfileRequest : IRequest<BaseResponse<bool>>
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;
}
public class EditAccountProfileRequestHandler(IValidator<EditAccountProfileRequest> validator, IIdentityService identity, ILogger<EditAccountProfileRequest> logger, IMapper mapper) : IRequestHandler<EditAccountProfileRequest, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(EditAccountProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"{nameof(EditAccountProfileRequestHandler)} started");
            var passwordHasher = new PasswordHasher<AppUser>();
            var result = await validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return new BaseResponse<bool>()
                {
                    Data = false,
                    Message = string.Join("\n ", result.Errors),
                    Success = false,
                    StatusCode = 422
                };
            }
            (string userId, string fullName, string UserName, string email, string phone, IList<string> roles) = await identity.GetUserDetails(request.Id);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
            {
                throw new KeyNotFoundException("User details not found");
            }

            var userToUpdate = mapper.Map<AppUser>((userId, fullName, email, phone, roles)) ?? throw new NullReferenceException("User not retrieved");

            userToUpdate.Email = request.Email;
            userToUpdate.FullName = request.Name;
            userToUpdate.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrEmpty(request.Password) && !string.IsNullOrWhiteSpace(request.Password) || !string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrWhiteSpace(request.OldPassword))
            {
                var resetPasswordResult = await identity.ResetPasswordAsync(request.Id, request.OldPassword, request.Password);
                if (!resetPasswordResult)
                {
                    return new BaseResponse<bool>()
                    {
                        Data = false,
                        Message = "Error when resetting password",
                        Success = false,
                        StatusCode = 400
                    };
                }
            }
            var updated = await identity.UpdateUserProfileAsync(userToUpdate.Id, userToUpdate.FullName, userToUpdate.Email, userToUpdate.PhoneNumber, userToUpdate.Roles.Select(x => x.Name).ToList()!);
            return new BaseResponse<bool>()
            {
                Data = updated,
                Success = updated,
                Message = updated ? "User successfully updated" : "User not updated",
                StatusCode = updated ? 200 : 400
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(EditAccountProfileRequestHandler)} finishes");
        }
    }
}
