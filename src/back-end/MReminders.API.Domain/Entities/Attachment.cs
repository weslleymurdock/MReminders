using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MReminders.API.Domain.Entities;
[PrimaryKey(nameof(Id))]

public class Attachment : BaseEntity
{
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
    [ForeignKey(nameof(ReminderId))]
    public Reminder Reminder { get; set; } = default!;
}
