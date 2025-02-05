using MReminders.Rest.Client;

namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IRemindersService
{
    Task<ReminderResponseIEnumerableBaseResponse> GetRemindersFromUserAsync(string userId, CancellationToken token);
    Task<ReminderResponseBaseResponse> AddReminderAsync(AddReminderRequest request, CancellationToken token);
    Task<ReminderResponseBaseResponse> EditReminderAsync(EditReminderRequest request, CancellationToken token);
    Task<BooleanBaseResponse> DeleteReminderAsync(DeleteReminderRequest request, CancellationToken token);

    //Attachments

    Task<AttachmentResponseIEnumerableBaseResponse> GetAttachmentsFromReminderAsync(string reminderId, CancellationToken token);
    Task<AttachmentResponseBaseResponse> AddAttachmentAsync(AddAttachmentRequest request, CancellationToken token);
    Task<AttachmentResponseBaseResponse> EditAttachmentAsync(EditAttachmentRequest request, CancellationToken token);
    Task<BooleanBaseResponse> DeleteAttachmentAsync(DeleteAttachmentRequest request, CancellationToken token);
}
