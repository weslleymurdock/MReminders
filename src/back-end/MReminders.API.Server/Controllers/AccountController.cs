using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Application.Requests.Reminders;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Base;
using System.Text;

namespace MReminders.API.Server.Controllers;
/// <summary>
/// Controlador de API Rest para Contas de usuário
/// </summary>
/// <param name="mediator">instancia do mediator para execução das requisições</param>
/// <param name="logger">instancia de logger para o controlador</param>
[ApiController]
[Route("api/[controller]")]
public class AccountController(IMediator mediator, ILogger<AccountController> logger) : ControllerBase
{

    /// <summary>
    /// Endpoint para consulta de um unico usuario informando a chave
    /// </summary>
    /// <param name="key">Chave de consulta para encontrar o usuario, sendo a chave um dos seguintes valores: Id do usuario, username, email ou numero de telefone</param>
    /// <returns>
    /// Objeto do tipo <see cref="BaseResponse{T}" />com os dados do usuario envelopados
    /// </returns>
    /// <response code="200">Retorna o usuário relacionado à busca</response>
    /// <response code="400">Algum erro interno ocorreu</response>
    /// <response code="401">Um erro de autenticação ocorreu</response>
    /// <response code="401">Um objeto para a busca não foi encontrado</response>
    /// <remarks>
    /// Este endpoint permite realizar a busca por um usuário utilizando diferentes chaves de consulta.
    ///
    /// Exemplos de chamadas:
    /// - Buscar por Id do usuário: `GET /GetUser/12345`
    /// - Buscar por username: `GET /GetUser/johndoe`
    /// - Buscar por email: `GET /GetUser/johndoe@example.com`
    /// - Buscar por número de telefone: `GET /GetUser/+1234567890`
    /// 
    /// Exemplo de header necessário para a chamada:
    /// 
    /// ```
    /// GET /GetUser/12345 HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// /// <returns>Objeto do tipo 
    /// Content-Type: application/json
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{UserResponse}` que contém os dados do usuário encontrado ou mensagens de erro apropriadas.
    [HttpGet("GetUser/{key}", Name = "GetUser")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<UserResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<UserResponse>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<LoginResponse>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<UserResponse>))]
    [ProducesErrorResponseType(typeof(BaseResponse<UserResponse>))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetUser(string key)
    {
        try
        {
            var request = new GetUserAccountRequest() { UserKey = key };
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
            return Unauthorized(new BaseResponse<UserResponse> { Data = { }, Message = $"Unauthorized access: {e.Message}", StatusCode = 401, Success = false });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<UserResponse> { Data = { }, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Endpoint para realizar requisições de Login (autenticação). Utiliza o metodo de autenticação Basic
    /// </summary>
    /// <returns>
    /// Response do tipo <see cref="BaseResponse{LoginResponse}" /> com token presente se a requisição for bem sucedida. 
    /// Se não, retorna a response com o respectivo erro
    /// </returns>
    /// <remarks>
    /// Este endpoint realiza login utilizando autenticação Basic. 
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// POST /Login HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Basic base64encode(username:password)
    /// Content-Type: application/json
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{LoginResponse}` contendo o token de autenticação ou mensagens de erro apropriadas.
    /// </remarks>
    [HttpPost("Login", Name = "Login")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<LoginResponse>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<LoginResponse>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<LoginResponse>))]
    [ProducesErrorResponseType(typeof(BaseResponse<LoginResponse>))]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<IActionResult> Login([FromHeader(Name = "Authorization")] string basic)
    {
        try
        {
            var credentials = Encoding.UTF8
               .GetString(Convert.FromBase64String(basic!.Replace("Basic ", "") ?? string.Empty))
               .Split(':', 2);

            var result = await mediator.Send(new LoginAccountRequest() { Key = credentials[0], Password = credentials[1] });
            if (result == null)
            {
                return BadRequest(new BaseResponse<LoginResponse> { Data = {}, Message = $"There was an error", StatusCode = 400, Success = false });
            }

            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
                401 => Unauthorized(result),
                400 => BadRequest(result),
                200 => Ok(result),
                _ => BadRequest(result),
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(new BaseResponse<LoginResponse> { Data = {}, Message = $"Unauthorized access: {e.Message}", StatusCode = 401, Success = false });
        } 
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<LoginResponse> { Data = { }, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Endpoint para cadastro de um usuario na base de dados. Não requer autenticação.
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="RegisterAccountRequest"/> com os dados do usuario a ser cadastrado</param>
    /// <returns></returns>
    /// <response code="201">Conta do usuário criada com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="409">Conflito - o email ou nome de usuário já está em uso.</response>
    /// <response code="422">Dados fornecidos não são passíveis de processamento.</response>
    /// <remarks>
    /// Este endpoint permite registrar uma nova conta de usuário.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// POST /Register HTTP/1.1
    /// Host: mreminders.local
    /// Content-Type: application/json
    ///
    /// {
    ///     "name": "John Doe",
    ///     "email": "john.doe@example.com",
    ///     "userName": "johndoe",
    ///     "password": "Password123!",
    ///     "phoneNumber": "+1234567890",
    ///     "confirmationPassword": "Password123!",
    ///     "roles": ["user"]
    /// }
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{UserResponse}` contendo os dados do usuário recém-criado ou mensagens de erro apropriadas.
    /// </remarks>
    [HttpPost("Register", Name = "Register")]
    [ProducesResponseType(201, Type = typeof(BaseResponse<UserResponse>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<UserResponse>))] 
    [ProducesResponseType(409, Type = typeof(BaseResponse<UserResponse>))]
    [ProducesResponseType(422, Type = typeof(BaseResponse<UserResponse>))]
    [ProducesErrorResponseType(typeof(BaseResponse<UserResponse>))]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
    {
        try
        {
            var result = await mediator.Send(request);
            if (result == null)
            {
                return BadRequest(new BaseResponse<UserResponse> { Data = {}, Message = $"There was an error", StatusCode = 400, Success = false });
            }

            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
                409 => Conflict(result),
                400 => BadRequest(result),
                201 => Created("api/Account/Register", result),
                _ => BadRequest(result),
            };
        } 
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return BadRequest(new BaseResponse<UserResponse> { Data = { }, Message = $"There was an error: {e.Message}", StatusCode = 400, Success = false });
        }
    }

    /// <summary>
    /// Edita os dados do usuario selecionado
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="EditAccountProfileRequest"/> com os dados do usuario para atualizar</param>
    /// <returns>Response do tipo <see cref="BaseResponse{T}" /> informando o resultado da operação atraves de um valor booleano</returns>
    /// <response code="200">Os dados do usuário foram atualizados com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    /// <remarks>
    /// Este endpoint permite editar os dados de um usuário existente.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// PUT /UpdateProfile HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// Content-Type: application/json
    ///
    /// {
    ///     "id": "1a7bfa12-be25-41cc-9a07-884543d7f234",
    ///     "name": "Jane Doe",
    ///     "email": "jane.doe@example.com",
    ///     "phoneNumber": "+5511923456789",
    ///     "password": "NewPassword123!",
    ///     "oldPassword": "OldPassword123!"
    /// }
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{bool}` que indica o resultado da operação.
    /// </remarks>
    [HttpPut("UpdateProfile", Name = "UpdateProfile")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<bool>))]
    [ProducesErrorResponseType(typeof(BaseResponse<bool>))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateProfile([FromBody] EditAccountProfileRequest request)
    {
        try
        {
            var result = await mediator.Send(request);
            if (result == null)
            {
                return BadRequest(new BaseResponse<bool> { Data = false, Message = $"There was an error", StatusCode = 400, Success = false });
            }

            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
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
    /// Exclui os dados do usuario selecionado
    /// </summary>
    /// <param name="request">Objeto do tipo <see cref="EditAccountProfileRequest"/> com os dados do usuario para atualizar</param>
    /// <returns>Response do tipo <see cref="BaseResponse{T}" /> informando o resultado da operação atraves de um valor booleano</returns>
    /// <response code="200">Os dados do usuário foram excluidos com sucesso.</response>
    /// <response code="400">Algum erro na requisição ou validação dos dados.</response>
    /// <response code="401">Acesso não autorizado.</response>
    /// <response code="404">Usuário não encontrado.</response>
    /// <remarks>
    /// Este endpoint permite excluir os dados de um usuário existente.
    ///
    /// Exemplo de chamada:
    ///
    /// ```
    /// DELETE /DeleteUser HTTP/1.1
    /// Host: mreminders.local
    /// Authorization: Bearer [token]
    /// Content-Type: application/json
    ///
    /// {
    ///     "key": "1a7bfa12-be25-41cc-9a07-884543d7f234"
    /// }
    /// ```
    /// 
    /// O endpoint retorna um objeto `BaseResponse{bool}` que indica o resultado da operação.
    /// </remarks>
    [HttpDelete("DeleteUser", Name = "DeleteUser")]
    [ProducesResponseType(200, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(401, Type = typeof(BaseResponse<bool>))]
    [ProducesResponseType(404, Type = typeof(BaseResponse<bool>))]
    [ProducesErrorResponseType(typeof(BaseResponse<bool>))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteAccountRequest request)
    {
        try
        {
            var result = await mediator.Send(request);
            if (result == null)
            {
                return BadRequest(new BaseResponse<bool> { Data = false, Message = $"There was an error", StatusCode = 400, Success = false });
            }

            return result.StatusCode switch
            {
                422 => UnprocessableEntity(result),
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


}
