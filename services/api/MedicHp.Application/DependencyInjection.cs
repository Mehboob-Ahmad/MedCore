using System.Reflection;
using FluentValidation;
using MediatR;
using MedicHp.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace MedicHp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddScoped<MedicHp.Application.Features.Auth.Interfaces.IAuthService, MedicHp.Application.Features.Auth.Services.AuthService>();
        services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<MedicHp.Domain.Entities.Core.User>, Microsoft.AspNetCore.Identity.PasswordHasher<MedicHp.Domain.Entities.Core.User>>();

        return services;
    }
}
