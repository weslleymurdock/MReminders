using MReminders.API.Domain.Entities;
using System.Linq.Expressions;

namespace MReminders.API.Infrastructure.Interfaces;

public interface IReminderService
{
    Task<Attachment> AddAttachment(Attachment attachment);
    Task<bool> EditAttachment(Attachment attachment);
    Task<bool> DeleteAttachment(Attachment attachment);
    Attachment GetAttachment(Expression<Func<Attachment, bool>> expression);
    IQueryable<Attachment> GetAttachments();
    Task<Reminder> AddReminder(Reminder reminder);
    Task<bool> EditReminder(Reminder reminder);
    Task<bool> DeleteReminder(Reminder reminder);
    Reminder GetReminder(Func<Reminder, bool> expression);
    IEnumerable<Reminder> GetReminders();
    IEnumerable<Reminder> GetRemindersFromUser(string userId);


}
