using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MReminders.API.Application.Mappings;
using MReminders.API.Application.Validators.Account;
using MReminders.API.Infrastructure;
namespace MReminders.API.Application
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddValidatorsFromAssemblyContaining<AccountLoginRequestValidator>();
            services.AddInfrastructure(configuration);
            return services;
        }
    }
}
