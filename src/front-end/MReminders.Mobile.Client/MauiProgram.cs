
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Application;
using MReminders.Mobile.Infrastructure.Handlers;
using MReminders.Rest.Client; 

namespace MReminders.Mobile.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.AddMauiBase();
            builder.AddViewsAndViewModels();
            builder.AddPopups();
            //builder.AddCertificate();
            builder.AddHandlers();
            builder.Services.AddHttpClient("MRemindersClient", (sp, options) =>
            {
                options.BaseAddress = new(builder.Configuration.GetRequiredSection("API:BaseUrl").Value!);
            })
            .ConfigurePrimaryHttpMessageHandler(sp => new IgnoreCertificateValidationHandler());
             
            builder.Services.AddTransient(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("MRemindersClient");
                var baseUrl = builder.Configuration.GetRequiredSection("API:BaseUrl").Value;
                return new MRemindersClient(baseUrl, httpClient);
            });
            builder.AddApplication();
            return builder.Build();
        }
    }
}
