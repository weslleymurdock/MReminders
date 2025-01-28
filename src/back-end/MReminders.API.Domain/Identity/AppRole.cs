using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MReminders.API.Domain.Identity;
[PrimaryKey(nameof(Id))]

public class AppRole : IdentityRole<string>
{
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
