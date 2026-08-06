using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedCore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MedCore.Infrastructure.Settings.JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<MedCore.Application.Features.Auth.Interfaces.ITokenService, MedCore.Infrastructure.Services.Auth.TokenService>();
        services.AddScoped<MedCore.Application.Features.Auth.Interfaces.ICurrentUserService, MedCore.Infrastructure.Services.Auth.CurrentUserService>();
        services.AddScoped<MedCore.Application.Features.Auth.Interfaces.IEmailService, MedCore.Infrastructure.Services.Auth.EmailService>();

        // HttpContextAccessor is needed by CurrentUserService
        services.AddHttpContextAccessor();

        return services;
    }
}
