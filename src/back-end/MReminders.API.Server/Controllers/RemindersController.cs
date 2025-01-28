using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Application.Requests.Reminders;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Application.Responses.Reminders;

namespace MReminders.API.Server.Controllers;
/// <summary>
/// Controlador de lembretes.
/// </summary>
/// <param name="mediator">Instância do mediator</param>
/// <param name="logger">Instancia do logger para o controller</param>
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RemindersController(IMediator mediator, ILogger<RemindersController> logger) : ControllerBase
{
    /// <summary>
    /// Adiciona um lembrete para um usuário
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="AddReminderRequest"/> que vem no body da requisição</param>
    /// <returns>Objeto do tipo <see cref="BaseResponse{T}"/> com o resultado da operação, onde <typeparamref name="T" /> é <see cref="ReminderResponse"/> </returns>
    [HttpPost("AddReminder", Name = "AddReminder")]
    [ProducesResponseType(201, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(409, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesErrorResponseType(typeof(BaseResponse<ReminderResponse>))]
    public async Task<IActionResult> AddReminder([FromBody] AddReminderRequest request)
    {
        try
        {
            var result = await mediator.Send(request);
            if (result == null)
            {
                return BadRequest(new BaseResponse<ReminderResponse> { Data = { }, Message = $"There was an error", StatusCode = 400, Success = false });
            }

            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
                409 => Conflict(result),
                401 => Unauthorized(result),
                400 => BadRequest(result),
                201 => Created("api/Reminders/AddReminder", result),
                _ => BadRequest(result),
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new BaseResponse<bool> { Data = false, Message = $"Unauthorized access: {e.Message}", StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<ReminderResponse> { Data = { }, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }


    /// <summary>
    /// Exclui um lembrete a partir de uma palavra chave (Username, Email, Telefone)
    /// </summary>
    /// <param name="request">json contendo a chave de busca: Username ou email ou numero de telefone </param>
    /// <returns></returns>
    [HttpDelete("DeleteReminder/{request}", Name = "DeleteReminder")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<bool>))]
    [ProducesErrorResponseType(typeof(BaseResponse<bool>))]
    public async Task<IActionResult> DeleteReminder([FromBody] DeleteReminderRequest request)
    {
        try
        {
            var result = await mediator.Send(request);
            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
                409 => Conflict(result),
                404 => NotFound(result),
                400 => BadRequest(result),
                200 => Ok(result),
                _ => BadRequest(result),
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new BaseResponse<bool> { Data = false, Message = $"Unauthorized access: {e.Message}", StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<bool> { Data = false, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }


    /// <summary>
    /// Busca por lembretes de um usuário a partir de uma palavra-chave (Username, Email, Telefone)
    /// </summary>
    /// <param name="key">Chave de busca: Username ou email ou número de telefone </param>
    /// <returns>Um objeto do tipo <see cref="BaseResponse{T}"/> onde T é <see cref="IEnumerable{ReminderResponse}"/></returns>
    [HttpGet("GetRemindersByKey/{key}", Name = "GetReminders")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<IEnumerable<ReminderResponse>>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<IEnumerable<ReminderResponse>>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<IEnumerable<ReminderResponse>>))]
    [ProducesErrorResponseType(typeof(BaseResponse<IEnumerable<ReminderResponse>>))]
    public async Task<IActionResult> GetRemindersByKey(string key)
    {
        try
        {
            var request = new GetRemindersFromUserRequest() { UserKey = key };
            var result = await mediator.Send(request);
            return result.StatusCode switch
            {
                404 => NotFound(result),
                400 => BadRequest(result),
                200 => Ok(result),
                _ => BadRequest(result),
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new BaseResponse<bool> { Data = false, Message = $"Unauthorized access: {e.Message}", StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<IEnumerable<ReminderResponse>> { Data = { }, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Edita um lembrete de um usuario
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="EditReminderRequest"/> que vem no body da request</param>
    /// <returns>Um objeto do tipo <see cref="BaseResponse{T}"/> contendo o resultado da operação onde <typeref name="T"/> é <see cref="ReminderResponse"/></returns>
    [ProducesResponseType(200, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<ReminderResponse>))]
    [ProducesErrorResponseType(typeof(BaseResponse<ReminderResponse>))]
    [HttpPut("UpdateReminder", Name = "UpdateReminder")]
    public async Task<IActionResult> UpdateReminder([FromBody] EditReminderRequest request)
    {
        try
        {
            var result = await mediator.Send(request);
            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
                404 => NotFound(result),
                400 => BadRequest(result),
                200 => Ok(result),
                _ => BadRequest(result)
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new BaseResponse<bool> { Data = false, Message = $"Unauthorized access: {e.Message}", StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<ReminderResponse> { Data = { }, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }
}
