using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MReminders.Mobile.Domain.Entities;
namespace MReminders.Mobile.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>  
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        SQLitePCL.Batteries_V2.Init();
        this.Database.EnsureCreated();
    }
}
