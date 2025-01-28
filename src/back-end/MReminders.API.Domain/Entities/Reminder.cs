using Microsoft.EntityFrameworkCore;
using MReminders.API.Domain.Enums;
using MReminders.API.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MReminders.API.Domain.Entities;
[PrimaryKey(nameof(Id))]

public class Reminder : BaseEntity
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = default!;
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    [Required]
    public DateTime DueDate { get; set; }
    public bool OverDue { get => DueDate <= DateTime.Now; }
    public bool Done { get; set; }
    public bool Repeat { get; set; }
    public RepetitionType Repetition { get; set; }
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
