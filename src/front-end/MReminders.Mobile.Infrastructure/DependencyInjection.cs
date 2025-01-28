using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MReminders.Mobile.Domain.Entities;
using MReminders.Mobile.Infrastructure.Data;
using System.Text;

namespace MReminders.Mobile.Infrastructure;

public static class DependencyInjection
{
    public static MauiAppBuilder AddInfrastructure(this MauiAppBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "mreminders.db3");
            options.UseSqlite($"Data Source={dbPath}", sqlite =>
            {
                sqlite.CommandTimeout(600);
                sqlite.MigrationsAssembly(typeof(DependencyInjection).Assembly);
            });
        });

        builder.Services.AddIdentity<AppUser, AppRole>()
                 .AddEntityFrameworkStores<AppDbContext>()
                 .AddDefaultTokenProviders();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = $"/Account.Login";
            options.LogoutPath = $"/Account.Logout";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        });
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1d15f327a58d4a39887de5149dae09c3")),
                ClockSkew = TimeSpan.FromHours(2)
            };
        });
        builder.Services.AddTransient<SignInManager<AppUser>>();
        builder.Services.AddTransient<UserManager<AppUser>>();

        return builder;
    }
}
