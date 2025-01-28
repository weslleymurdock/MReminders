using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MReminders.API.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MReminders.API.Domain.Identity;
[PrimaryKey(nameof(Id))]

public class AppUser : IdentityUser
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    public ICollection<AppRole> Roles { get; set; } = new List<AppRole>();
}