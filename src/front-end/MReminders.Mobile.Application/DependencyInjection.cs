using FluentValidation;
using MReminders.Mobile.Application.Mappings;
using MReminders.Mobile.Application.Validators;
using MReminders.Mobile.Infrastructure;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application;

public static class DependencyInjection
{
    public static MauiAppBuilder AddApplication(this MauiAppBuilder builder)
    {
        builder.AddMediatR();
        builder.AddAutoMapper(); 
        builder.AddFluentValidation();
        builder.AddInfrastructure();
        return builder;
    }

    private static MauiAppBuilder AddMediatR(this MauiAppBuilder builder)
    {
        builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return builder;
    }
    
    private static MauiAppBuilder AddAutoMapper(this MauiAppBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        return builder;
    }

     
    private static MauiAppBuilder AddFluentValidation(this MauiAppBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        return builder;
    }

}
