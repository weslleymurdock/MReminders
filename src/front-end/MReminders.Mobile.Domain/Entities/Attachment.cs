namespace MReminders.Mobile.Domain.Entities;

public class Attachment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
}
