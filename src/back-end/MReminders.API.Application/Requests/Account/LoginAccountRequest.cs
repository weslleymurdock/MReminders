using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace MReminders.API.Application.Requests.Account;

public class LoginAccountRequest : IRequest<BaseResponse<LoginResponse>>
{
    public string Key { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class LoginAccountRequestHandler(IValidator<LoginAccountRequest> validator, IIdentityService identity, ILogger<LoginAccountRequest> logger, IConfiguration configuration) : IRequestHandler<LoginAccountRequest, BaseResponse<LoginResponse>>
{
    public async Task<BaseResponse<LoginResponse>> Handle(LoginAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"{nameof(EditAccountProfileRequestHandler)} started");

            var result = await validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return new BaseResponse<LoginResponse>()
                {
                    Data = null!,
                    Message = string.Join("\n ", result.Errors),
                    Success = false,
                    StatusCode = 422
                };
            }

            var hasSignedIn = await identity.SigninUser(request.Key, request.Password);
            if (!hasSignedIn)
            {
                return new BaseResponse<LoginResponse>()
                {
                    Data = null!,
                    Message = "Invalid user or password",
                    Success = false,
                    StatusCode = 401
                };
            }
            var (userId, fullName, UserName, email, phone, roles) = await identity.GetUserDetails(request.Key);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
            var expires = DateTime.UtcNow.AddHours(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, request.Key),
                    new Claim(ClaimTypes.Expiration, expires.ToString()),
                    new Claim(ClaimTypes.Role, string.Join(";", roles))
                ]),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwt);
             
            return new BaseResponse<LoginResponse>()
            {
                Data = new LoginResponse()
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Token = token,
                    Roles = roles,
                    Id = userId,
                    ExpirationDate = tokenDescriptor.Expires.Value
                },
                Message = "Login successful",
                Success = true,
                StatusCode = 200
            };

        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new BaseResponse<LoginResponse>()
            {
                Data = null!,
                Message = $"There was a error: {e.Message}",
                Success = false,
                StatusCode = 400
            };
        }
        finally
        {
            logger.LogInformation($"{nameof(EditAccountProfileRequestHandler)} finishes");
        }
    }
}
