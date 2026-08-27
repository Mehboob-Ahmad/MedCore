using MedicHp.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicHp.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("MedicHp.Persistence")));

        services.AddScoped(typeof(MedicHp.Application.Common.IGenericRepository<>), typeof(MedicHp.Persistence.Repositories.GenericRepository<>));
        services.AddScoped<MedicHp.Application.Common.IUnitOfWork, MedicHp.Persistence.Repositories.UnitOfWork>();

        return services;
    }
}
