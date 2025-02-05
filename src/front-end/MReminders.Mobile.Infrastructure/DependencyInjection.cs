using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MReminders.Mobile.Domain.Entities;
using MReminders.Mobile.Infrastructure.Data;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Mobile.Infrastructure.Repositories;
using MReminders.Mobile.Infrastructure.Services;
using MReminders.Mobile.Infrastructure.Storages;
using System.Text;

namespace MReminders.Mobile.Infrastructure;

public static class DependencyInjection
{
    public static MauiAppBuilder AddInfrastructure(this MauiAppBuilder builder)
    {
        builder.AddStorages();
        builder.AddDbContext();
        builder.AddRepositories();
        builder.AddServices();

        return builder;
    }

    private static MauiAppBuilder AddStorages(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton(typeof(IProtectedStorage<>), typeof(ProtectedStorage<>));
        builder.Services.AddSingleton<ITokenStorage, TokenStorage>();
        return builder;
    }

    private static MauiAppBuilder AddRepositories(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        return builder;
    }

    private static MauiAppBuilder AddDbContext(this MauiAppBuilder builder)
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
        return builder;
    }



    private static MauiAppBuilder AddServices(this MauiAppBuilder builder)
    {
        builder.Services.AddScoped<UserManager<AppUser>>();
        builder.Services.AddScoped<IBiometricsService, BiometricsService>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IPermissionsService, PermissionsService>();
        builder.Services.AddScoped<ITokenRenewalService, TokenRenewalService>();
        builder.Services.AddScoped<IRemindersService, ReminderService>();
        return builder;
    }
}
