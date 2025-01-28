using MReminders.API.Application;
using MReminders.API.Server;
using MReminders.API.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer; 

var builder = WebApplication.CreateBuilder(args);

builder
    .AddBasicBearerAuth()
    .AddApplication(builder.Configuration)
    .AddOutputCache(options =>
    {
        options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
    })
    .AddControllers()
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "MReminders API",
            Description = "An ASP.NET Core Web API for managing reminders",
            Contact = new OpenApiContact
            {
                Name = "Weslley Luiz",
                Url = new Uri("https://weslleymurdock.github.io/contact")
            }
        });
        var bearerSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Insira 'Bearer' [espaço] e então seu token JWT",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition(bearerSecurityScheme.Reference.Id, bearerSecurityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { bearerSecurityScheme, Array.Empty<string>() }
        });

        var basicSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "Basic",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = "Insira seu login e senha",
            Reference = new OpenApiReference
            {
                Id = "Basic",
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition(basicSecurityScheme.Reference.Id, basicSecurityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { basicSecurityScheme, Array.Empty<string>() }
        });
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename)); 
    });

var app = builder.Build();

app.MigrateDatabase<AppDbContext>();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
        .UseSwaggerUI();
}

app.UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();
