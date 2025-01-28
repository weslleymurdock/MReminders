using Microsoft.EntityFrameworkCore;

namespace MReminders.API.Domain.Entities;
 
public class BaseEntity
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
