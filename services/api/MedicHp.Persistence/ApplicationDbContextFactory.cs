using MedicHp.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MedicHp.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // This is only used for design-time tools (e.g., dotnet ef migrations add)
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // For design time, we just need a valid connection string to PostgreSQL
        // Assuming a standard local postgres instance for design time
        var connectionString = "Host=localhost;Database=MedicHp;Username=postgres;Password=postgres";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options, new AuditableEntityInterceptor());
    }
}
