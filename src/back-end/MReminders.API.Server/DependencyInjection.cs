using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MReminders.API.Infrastructure.Handlers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MReminders.API.Domain.Identity;

namespace MReminders.API.Server;
/// <summary>
/// Classe de extensão para injeção de dependências
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona Autenticação basic and bearer ao container de injeção de dependência.
    /// </summary>
    /// <param name="builder">Web Application Builder</param>
    /// <returns>Coleção de serviços do tipo <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddBasicBearerAuth(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.FromMinutes(System.Convert.ToDouble(builder.Configuration["Jwt:ExpiryMinutes"]))
                };
            })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options => { });

        return builder.Services;
    }

    /// <summary>
    /// Metodo para executar migrações no banco de dados
    /// </summary>
    /// <typeparam name="TContext">Tipo do contexto do banco de dados a ser migrado</typeparam>
    /// <param name="app">web application</param>
    public static void MigrateDatabase<TContext>(this WebApplication app) where TContext : IdentityDbContext<AppUser, AppRole, string>
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<TContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<TContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
