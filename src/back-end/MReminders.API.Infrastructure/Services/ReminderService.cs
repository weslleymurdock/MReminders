using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
using MReminders.API.Domain.Entities;
using MReminders.API.Infrastructure.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace MReminders.API.Infrastructure.Services;

public class ReminderService(IRepository<Reminder> reminderRepository, IRepository<Attachment> attachmentsRepository, ILogger<ReminderService> logger) : IReminderService
{
    public async Task<Attachment> AddAttachment(Attachment attachment)
    {
        try
        {
            return await attachmentsRepository.CreateAsync(attachment);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> EditAttachment(Attachment attachment)
    {
        try
        {
            return await attachmentsRepository.EditAsync(attachment);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    
    public async Task<bool> DeleteAttachment(Attachment attachment)
    {
        try
        {
            return await attachmentsRepository.EditAsync(attachment);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    public Attachment GetAttachment(Expression<Func<Attachment, bool>> expression)
    {
        try
        {
            return GetAttachments().Where(expression).SingleOrDefault() ?? default!;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public IQueryable<Attachment> GetAttachments()
    {
        try
        {
            return attachmentsRepository.Get(x => x.Reminder);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    public async Task<Reminder> AddReminder(Reminder reminder)
    {
        try
        {
            return await reminderRepository.CreateAsync(reminder);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteReminder(Reminder reminder)
    {
        try
        {
            return await reminderRepository.DeleteAsync(reminder);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> EditReminder(Reminder reminder)
    {
        try
        {
            return await reminderRepository.EditAsync(reminder);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    public Reminder GetReminder(Func<Reminder, bool> expression)
    {
        try
        {
            return reminderRepository.Get(x => x.Attachments).Where(expression).FirstOrDefault() ?? throw new KeyNotFoundException("Reminders not found for user");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
     
    public IEnumerable<Reminder> GetReminders()
    {
        try
        {
            return reminderRepository.Get(x => x.Attachments);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
    public IEnumerable<Reminder> GetRemindersFromUser(string userId)
    {
        try
        {
            return reminderRepository.Get(x => x.Attachments).Where(x => x.UserId == userId) ?? throw new KeyNotFoundException("Reminders not found for user");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
