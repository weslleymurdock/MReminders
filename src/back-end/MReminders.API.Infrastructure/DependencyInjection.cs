using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MReminders.API.Domain.Identity;
using MReminders.API.Infrastructure.Data;
using MReminders.API.Infrastructure.Interfaces;
using MReminders.API.Infrastructure.Repository;
using MReminders.API.Infrastructure.Services;
using System.Text;

namespace MReminders.API.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            //var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            options.UseSqlServer(configuration.GetConnectionString("DockerConnection"), dbOptions =>
            {
                dbOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                dbOptions.EnableRetryOnFailure();
                dbOptions.CommandTimeout(6000);
            });
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });
        services.AddIdentity<AppUser, AppRole>()
           .AddEntityFrameworkStores<AppDbContext>() 
           .AddDefaultTokenProviders();

         
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IReminderService, ReminderService>();
        return services;
    }
}
