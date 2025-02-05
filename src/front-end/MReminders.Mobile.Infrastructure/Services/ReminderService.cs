using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Infrastructure.Services;

internal class ReminderService(MRemindersClient client, ITokenStorage storage, ILogger<ReminderService> logger) : IRemindersService
{
    public async Task<AttachmentResponseBaseResponse> AddAttachmentAsync(AddAttachmentRequest request, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(AddAttachmentAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            var response = await client.AddAttachmentAsync(request, token);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(AddAttachmentAsync)} ends");
        }
    }

    public async Task<ReminderResponseBaseResponse> AddReminderAsync(AddReminderRequest request, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(AddReminderAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.AddReminderAsync(request, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(AddReminderAsync)} ends");
        }
    }

    public async Task<BooleanBaseResponse> DeleteAttachmentAsync(DeleteAttachmentRequest request, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(DeleteAttachmentAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.DeleteAttachmentAsync(request, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(DeleteAttachmentAsync)} ends");
        }
    }

    public async Task<BooleanBaseResponse> DeleteReminderAsync(DeleteReminderRequest request, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(DeleteReminderAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.DeleteReminderAsync(request.ReminderId, request, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(DeleteReminderAsync)} ends");
        }
    }

    public async Task<AttachmentResponseBaseResponse> EditAttachmentAsync(EditAttachmentRequest request, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(EditAttachmentAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.UpdateAttachmentAsync(request, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(EditAttachmentAsync)} ends");
        }
    }

    public async Task<ReminderResponseBaseResponse> EditReminderAsync(EditReminderRequest request, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(EditReminderAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.UpdateReminderAsync(request, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(EditReminderAsync)} ends");
        }
    }

    public async Task<AttachmentResponseIEnumerableBaseResponse> GetAttachmentsFromReminderAsync(string reminderId, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(GetAttachmentsFromReminderAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.GetAttachmentsByReminderAsync(reminderId, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(GetAttachmentsFromReminderAsync)} ends");
        }
    }

    public async Task<ReminderResponseIEnumerableBaseResponse> GetRemindersFromUserAsync(string userId, CancellationToken token)
    {
        try
        {
            logger.LogInformation($"{nameof(GetRemindersFromUserAsync)} starts");
            var bearerToken = await storage.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            client.BearerToken = bearerToken.Value;
            return await client.GetRemindersAsync(userId, token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(GetRemindersFromUserAsync)} ends");
        }
    }
}
