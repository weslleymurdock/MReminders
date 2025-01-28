namespace MReminders.API.Application.Responses.Attachment;

public class AttachmentResponse
{ 
    public string AttachmentId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
}
