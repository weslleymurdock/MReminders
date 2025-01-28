using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MReminders.Mobile.Domain.Entities;

[PrimaryKey(nameof(Id))]
public class AppUser : IdentityUser<string>
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
