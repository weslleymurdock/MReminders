namespace MReminders.Mobile.Application;

public static class DependencyInjection
{
    public static MauiAppBuilder AddApplication(this MauiAppBuilder builder)
    {
        builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return builder;
    }
}
