using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MReminders.API.Application.Requests.Attachments;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Application.Responses.Base;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MReminders.API.Server.Controllers;

/// <summary>
/// Controlador de anexos dos lembretes
/// </summary>
/// <param name="mediator">Instancia do mediator</param>
/// <param name="logger">Instancia do logger para o controlador</param>
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class AttachmentsController(IMediator mediator, ILogger<AttachmentsController> logger) : ControllerBase
{
    /// <summary>
    /// Exclui um anexo de um lembrete de um usuário
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Os dados do anexo foram excluidos com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="404">Anexo não encontrado.</response>
    /// <response code="409">Operação em conflito.</response>
    /// <response code="422">Os dados da entidade não são processáveis.</response>
    /// <remarks>
    /// Este endpoint permite excluir os dados de um anexo existente.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// DELETE /DeleteAttachment HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// Content-Type: application/json
    ///
    /// {
    ///     "attachmentId": "1a7bfa12-be25-41cc-9a07-884543d7f234",
    /// }
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{bool}` que indica o resultado da operação.
    /// </remarks>
    [HttpDelete("DeleteAttachment", Name = "DeleteAttachment")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(409, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<bool>))]
    public async Task<IActionResult> DeleteAttachment([FromBody] DeleteAttachmentRequest request)
    {
        try
        {
            var response = await mediator.Send(request);
            return response.StatusCode switch
            {
                422 => UnprocessableEntity(response),
                409 => Conflict(response),
                400 => BadRequest(response),
                200 => Ok(response),
                _ => BadRequest(response)
            };
        }
        catch (UnauthorizedAccessException e)
        {
            logger.LogError(e, e.Message);
            return Unauthorized(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 401, Success = false });
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(e, e.Message);
            return NotFound(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 404, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 400, Success = false });
        }
    }
    /// <summary>
    /// Retorna um anexos de um usuario atraves de um lembrete se baseando no nome do arquivo
    /// </summary>
    /// <returns></returns> 
    /// <response code="200">Os dados do anexo foram obtidos com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="404">Anexo não encontrado.</response>
    /// <remarks>
    /// Este endpoint permite obter os dados de um anexo existente atraves de um lembrete.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// GET /GetAttachmentsByReminder/1a7bfa12-be25-41cc-9a07-884543d7f234 HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{IEnumerable{AttachmentResponse}}` que indica o resultado da operação.
    /// </remarks>
    [HttpGet("GetAttachmentsByReminder/{reminderId}/")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<IEnumerable<AttachmentResponse>>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<IEnumerable<AttachmentResponse>>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<IEnumerable<AttachmentResponse>>))]

    public async Task<IActionResult> GetAttachmentsByReminder(string reminderId)
    {
        try
        {
            var request = new GetAttachmentByReminderRequest() { ReminderId = reminderId };
            var response = await mediator.Send(request);
            return response.StatusCode switch
            {
                404 => NotFound(response),
                400 => BadRequest(response),
                200 => Ok(response),
                _ => BadRequest(response)
            };
        }
        catch (UnauthorizedAccessException e)
        {
            logger.LogError(e, e.Message);
            return Unauthorized(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Retorna um anexos de um usuario atraves de um lembrete se baseando no nome do arquivo
    /// </summary>
    /// <returns></returns> 
    /// <response code="200">Os dados do anexo foram obtidos com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="404">Anexo não encontrado.</response>
    /// <remarks>
    /// Este endpoint permite obter os dados de anexos existentes com base no usuario e no nome do anexo.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// GET /GetAttachmentsByUser/6084523c-282c-419c-9dba-32128bea0c14/FileName/BackupAutomation.ps1 HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{IEnumerable{AttachmentResponse}}` que indica o resultado da operação.
    /// </remarks>
    [HttpGet("GetAttachmentsByUser/{userId}/FileName/{filename}/")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<IEnumerable<AttachmentResponse>>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<IEnumerable<AttachmentResponse>>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<IEnumerable<AttachmentResponse>>))]
    public async Task<IActionResult> GetAttachmentsByUserAndFilename(string userId, string filename)
    {
        try
        {
            var request = new GetAttachmentByKeysRequest() { FileName = filename, UserId = userId };
            var response = await mediator.Send(request);
            return response.StatusCode switch
            {
                404 => NotFound(response),
                400 => BadRequest(response),
                200 => Ok(response),
                _ => BadRequest(response)
            };
        }
        catch (UnauthorizedAccessException e)
        {
            logger.LogError(e, e.Message);
            return Unauthorized(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Adiciona um anexo em um lembrete  
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="AddAttachmentRequest"/> com os dados do anexo</param>
    /// <returns>Um objeto do tipo <see cref="BaseResponse{T}"/>com os dados da response, onde <typeparamref name="T" /> é <see cref="AttachmentResponse"/></returns>
    /// <response code="201">Os dados do anexo foram gravados com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="409">Operação em conflito.</response>
    /// <response code="422">Os dados da entidade não são processáveis.</response>
    /// <remarks>
    /// Este endpoint permite gravar os dados de um anexo.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// POST /AddAttachment HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// Content-Type: application/json
    ///
    /// {
    ///     "attachmentId": "1a7bfa12-be25-41cc-9a07-884543d7f234",
    /// }
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{bool}` que indica o resultado da operação.
    /// </remarks>
    [HttpPost("AddAttachment", Name = "AddAttachment")]
    [ProducesResponseType(201, Type = typeof(BaseResponse<AttachmentResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<AttachmentResponse>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(409, Type = typeof(BaseResponse<AttachmentResponse>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<AttachmentResponse>))]
    public async Task<IActionResult> AddAttachment([FromBody] AddAttachmentRequest request)
    {
        try
        {
            var response = await mediator.Send(request);
            return response.StatusCode switch
            {
                422 => UnprocessableEntity(response),
                409 => Conflict(response),
                400 => BadRequest(response),
                201 => Created("api/Attachments/AddAttachment/", response),
                _ => BadRequest(response)
            };
        }
        catch (UnauthorizedAccessException e)
        {
            logger.LogError(e, e.Message);
            return Unauthorized(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Edita um anexo de um lembrete
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="EditAttachmentRequest"/> presente no body da request</param> 
    /// <response code="200">Os dados do anexo foram atualizados com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="404">Anexo não encontrado.</response>
    /// <response code="409">Operação em conflito.</response>
    /// <response code="422">Os dados da entidade não são processáveis.</response>
    /// <remarks>
    /// Este endpoint permite atualizar os dados de um anexo existente.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// PUT /UpdateAttachment HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// Content-Type: application/json
    ///
    /// {
    ///     "attachmentId": "1a7bfa12-be25-41cc-9a07-884543d7f234",
    ///     "content": [],
    ///     "fileName": "BackupAutomation.ps1",
    ///     "contentType": "text/plain",
    ///     "ReminderId": "6084523c-282c-419c-9dba-32128bea0c14",
    /// }
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{AttachmentResponse}` que indica o resultado da operação.
    /// </remarks>
    [HttpPut("UpdateAttachment", Name = "UpdateAttachment")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<AttachmentResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<AttachmentResponse>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(409, Type = typeof(BaseResponse<AttachmentResponse>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<AttachmentResponse>))]
    public async Task<IActionResult> UpdateAttachment([FromBody] EditAttachmentRequest request)
    {
        try
        {
            var response = await mediator.Send(request);
            return response.StatusCode switch
            {
                422 => UnprocessableEntity(response),
                409 => Conflict(response),
                400 => BadRequest(response),
                200 => Ok(response),
                _ => BadRequest(response)
            };
        }
        catch (UnauthorizedAccessException e)
        {
            logger.LogError(e, e.Message);
            return Unauthorized(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 401, Success = false });
        }
        catch (KeyNotFoundException e)
        {
            logger.LogError(e, e.Message);
            return NotFound(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 404, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 400, Success = false });
        }
    }


}
