using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MReminders.API.Infrastructure.Handlers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MReminders.API.Domain.Identity;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Http;

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
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddSelfSignedCertificate(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpsRedirection(options =>
        {
            options.HttpsPort = options.GetHttpsPort();
        });

        // Carregar o certificado com chave privada
        var certificate =
        services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.ServerCertificate = httpsOptions.LoadCertificate(configuration.GetRequiredSection("Kestrel:Endpoints:Https:Certificate:Path").Value!, configuration.GetRequiredSection("Kestrel:Endpoints:Https:Certificate:KeyPath").Value!, configuration.GetRequiredSection("Certificate:Passphrase").Value!);
                ;
            });
        });
        return services;
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


    /// <summary>
    /// Adiciona o certificado presente no caminho informado e a chave privada presente no caminho informado, e importa os arquivos com a senha informada
    /// </summary>
    /// <param name="_">Extende o metodoo para as opções presentes em <see cref="HttpsConnectionAdapterOptions"/>.</param>
    /// <param name="certPath">caminho do certificado auto assinado (.pem)</param>
    /// <param name="keyPath">caminho da chave privada (.key)</param>
    /// <param name="password">senha da chave privada</param>
    /// <returns></returns>
    public static X509Certificate2 LoadCertificate(this HttpsConnectionAdapterOptions _, string certPath, string keyPath, string password)
    {
        // Leia o conteúdo dos arquivos PEM
        string certPem = File.ReadAllText(certPath);
        string keyPem = File.ReadAllText(keyPath);

        // Extraia o certificado do PEM
        var certBytes = GetPemBytes(certPem, "CERTIFICATE");
        var certificate = new X509Certificate2(certBytes);

        // Extraia a chave privada do PEM
        var rsa = RSA.Create();
        rsa.ImportFromEncryptedPem(keyPem, password);

        // Combine a chave privada com o certificado
        var certWithKey = certificate.CopyWithPrivateKey(rsa);

        return certWithKey;
    }
    private static byte[] GetPemBytes(string pem, string section)
    {
        var header = $"-----BEGIN {section}-----";
        var footer = $"-----END {section}-----";

        var start = pem.IndexOf(header, StringComparison.Ordinal) + header.Length;
        var end = pem.IndexOf(footer, start, StringComparison.Ordinal);

        var base64 = pem.Substring(start, end - start);
        return Convert.FromBase64String(base64);
    }
   
    /// <summary>
    /// Retorna a porta Https configurada na variavel de ambiente, ou 443 se nao existir uma variavel com a configuração da porta
    /// </summary>
    /// <param name="_">extensão para <see  cref="HttpsRedirectionOptions"/></param>
    /// <returns>Porta https</returns>
    public static int GetHttpsPort(this HttpsRedirectionOptions _)
    {
        return int.TryParse(Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"), out int port) ? port : 443;
    }
}
