using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Domain.Entities;

namespace MReminders.API.Application.Responses.Reminders;

public class ReminderResponse
{
    public string ReminderId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ReminderName { get; set; } = string.Empty;
    public string ReminderDescription { get; set; } = string.Empty;
    public string ReminderLocation { get; set; } = string.Empty;
    public DateTime ReminderDate { get; set; }
    public bool OverDue { get; set; }
    public bool Done { get; set; }
    public ICollection<AttachmentResponse> Attachments { get; set; } = [];
}
