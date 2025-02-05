using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MReminders.Mobile.Domain.Enums;

namespace MReminders.Mobile.Domain.Entities;

public class Reminder 
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
   
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool OverDue { get => DueDate <= DateTime.Now; }
    public bool Done { get; set; }
    public bool Repeat { get; set; }
    public RepetitionType Repetition { get; set; }
    public ICollection<Attachment> Attachments { get; set; } = [];
}