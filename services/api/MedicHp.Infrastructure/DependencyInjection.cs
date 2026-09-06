using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MedicHp.Application.Common;
using MedicHp.Infrastructure.Services;

namespace MedicHp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MedicHp.Infrastructure.Settings.JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<MedicHp.Application.Features.Auth.Interfaces.ITokenService, MedicHp.Infrastructure.Services.Auth.TokenService>();
        services.AddScoped<MedicHp.Application.Features.Auth.Interfaces.ICurrentUserService, MedicHp.Infrastructure.Services.Auth.CurrentUserService>();


        // HttpContextAccessor is needed by CurrentUserService
        services.AddHttpContextAccessor();
        
        services.AddHttpClient<IPushNotificationService, ExpoPushNotificationService>();

        // WhatsApp Services
        services.Configure<MedicHp.Infrastructure.Settings.WhatsAppSettings>(configuration.GetSection("WhatsApp"));
        services.Configure<MedicHp.Infrastructure.Settings.WhatsAppTemplateSettings>(configuration.GetSection("WhatsAppTemplates"));
        services.AddSingleton<IWhatsAppEventQueue, WhatsAppEventQueue>();
        // Register HttpClient for EmailService
        services.AddScoped<MedicHp.Application.Features.Auth.Interfaces.IEmailService, MedicHp.Infrastructure.Services.Auth.EmailService>();
        services.AddHttpClient<IWhatsAppService, WhatsAppService>();
        services.AddScoped<IWhatsAppNotificationService, WhatsAppNotificationService>();
        
        // Register HttpClient for AI Service
        services.AddHttpClient<MedicHp.Application.Features.AI.Interfaces.IAIAssistant, MedicHp.Infrastructure.Services.AI.GemmaAIService>();

        // Background Services
        services.AddHostedService<WhatsAppWebhookProcessor>();
        
        services.Configure<MedicHp.Infrastructure.Settings.NotificationSchedulerSettings>(
            configuration.GetSection("NotificationScheduler"));
        services.AddHostedService<AppointmentNotificationService>();

        return services;
    }
}
