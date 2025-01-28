using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MReminders.Mobile.Domain.Entities;
[PrimaryKey(nameof(Id))]
public class AppRole : IdentityRole<string>
{
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
