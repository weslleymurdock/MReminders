using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MReminders.API.Domain.Entities;
using MReminders.API.Domain.Enums;
using MReminders.API.Domain.Identity;

namespace MReminders.API.Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, string>(options)
{
    public DbSet<Reminder> Reminders { get; set; }
    public DbSet<Attachment> Attachments { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Reminders)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .IsRequired();

        modelBuilder.Entity<Reminder>()
            .HasMany(r => r.Attachments)
            .WithOne(a => a.Reminder)
            .HasForeignKey(a => a.ReminderId)
            .IsRequired();

        modelBuilder.Entity<Reminder>()
            .HasIndex(r => new { r.UserId, r.Name })
            .IsUnique(); 

        modelBuilder.Entity<Attachment>()
            .HasIndex(a => new { a.ReminderId, a.FileName })
            .IsUnique(); 

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.UserName)
            .IsUnique(); 

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique(); 
        
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique(); 

        modelBuilder.Entity<Reminder>()
            .Property(r => r.Repetition)
            .HasConversion<string>();
        
        SeedData(modelBuilder);
    }
    private void SeedData(ModelBuilder modelBuilder)
    {
        var passwordHasher = new PasswordHasher<AppUser>();

        // Roles
        var adminRole = new AppRole { Id = "e4e9c144-6f83-4462-9a42-e35b15f97ed4", Name = "admin", NormalizedName = "ADMIN" };
        var userRole = new AppRole { Id = "11526a74-0cd1-4185-b7b2-0113262784e2", Name = "user", NormalizedName = "USER" };

        modelBuilder.Entity<AppRole>().HasData(adminRole, userRole);

        // Users
        var sysAdmin = new AppUser
        {
            Id = "44881cc2-d9a2-4d8f-a9d5-07470eb66d52",
            UserName = "sysadmin",
            NormalizedUserName = "SYSADMIN",
            Email = "sysadmin@example.com",
            NormalizedEmail = "SYSADMIN@EXAMPLE.COM",
            PhoneNumber = "+5512987654321",
            FullName = "System Administrator", 
        };

        var alice = new AppUser
        {
            Id = "3b395038-e46a-4c82-b4b8-ae2152c841fa",
            UserName = "alice",
            NormalizedUserName = "ALICE",
            Email = "alice@example.com",
            NormalizedEmail = "ALICE@EXAMPLE.COM",
            PhoneNumber = "+5512987654322",
            FullName = "Alice User", 
        };

        var bob = new AppUser
        {
            Id = "0cf41d61-d061-4c1e-8352-5cb789713453",
            UserName = "bob",
            NormalizedUserName = "BOB",
            Email = "bob@example.com",
            NormalizedEmail = "BOB@EXAMPLE.COM",
            PhoneNumber = "+5512987654323",
            FullName = "Bob User", 
        };

        sysAdmin.PasswordHash = passwordHasher.HashPassword(sysAdmin, "5Y5#4dM1N");
        alice.PasswordHash = passwordHasher.HashPassword(alice, "4L1C3@abc");
        bob.PasswordHash = passwordHasher.HashPassword(bob, "B0b@!xyz");

        modelBuilder.Entity<AppUser>().HasData(sysAdmin, alice, bob);

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = sysAdmin.Id, RoleId = adminRole.Id },
            new IdentityUserRole<string> { UserId = alice.Id, RoleId = userRole.Id },
            new IdentityUserRole<string> { UserId = bob.Id, RoleId = userRole.Id }
        );

        // Reminders
        var reminder1 = new Reminder
        {
            Id = "a6a2cdac-72c5-4447-aef0-6fc29cb42e95",
            UserId = sysAdmin.Id,
            Name = "Daily Backup",
            Description = "Perform daily backup",
            DueDate = new DateTime(2025, 1, 26, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4316),
            Repetition = RepetitionType.Daily, 
        };

        var reminder2 = new Reminder
        {
            Id = "5019de8a-dc3c-4699-8410-ec54b4c6a15d",
            UserId = alice.Id,
            Name = "Weekly Meeting",
            Description = "Participate in weekly meeting",
            DueDate = new DateTime(2025, 2, 1, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4862),
            Repetition = RepetitionType.Weekly, 
        };

        var reminder3 = new Reminder
        {
            Id = "bbee338d-1dc6-4309-a859-e34b5236256d",
            UserId = bob.Id,
            Name = "Grocery Shopping",
            Description = "Buy groceries for the week",
            DueDate = new DateTime(2025, 1, 28, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4874),
            Repetition = RepetitionType.None, 
        };

        var reminder4 = new Reminder
        {
            Id = "4e9957d7-499d-42f9-a513-15e15767965e",
            UserId = bob.Id,
            Name = "Pay Utility Bills",
            Description = "Pay electricity and water bills",
            DueDate = new DateTime(2025, 1, 30, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4883),
            Repetition = RepetitionType.Monthly, 
        };

        var reminder5 = new Reminder
        {
            Id = "937a8fa6-c14b-4e17-a4cc-16aee9f7b322",
            UserId = alice.Id,
            Name = "Team Lunch",
            Description = "Have lunch with the team",
            DueDate = new DateTime(2025, 2, 8, 10, 52, 52, 241, DateTimeKind.Utc).AddTicks(4899),
            Repetition = RepetitionType.None, 
        };

        modelBuilder.Entity<Reminder>().HasData(reminder1, reminder2, reminder3, reminder4, reminder5);

        // Attachments
        var attachment1 = new Attachment
        {
            Id = "7390b5e4-ab20-4a05-adf1-a1ade898052d",
            FileName = "BackupAutomationScript.ps1",
            Content = new byte[0], // Você pode adicionar conteúdo específico
            ContentType = "text/x-powershell",
            ReminderId = reminder1.Id, 
        };

        var attachment2 = new Attachment
        {
            Id = "fb6a88c0-5f7f-4782-acec-bd2c2e7a584b",
            FileName = "GroceryList.pdf",
            Content = new byte[0], // Você pode adicionar conteúdo específico
            ContentType = "application/pdf",
            ReminderId = reminder3.Id, 
        };

        modelBuilder.Entity<Attachment>().HasData(attachment1, attachment2);
    }
}
