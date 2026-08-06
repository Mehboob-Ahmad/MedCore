using MedCore.Domain.Entities.Core;
using MedCore.Domain.Entities.Lookup;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1. Roles
        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role { Name = "SystemAdmin", NormalizedName = "SYSTEMADMIN", Description = "System Administrator" };
            var doctorRole = new Role { Name = "Doctor", NormalizedName = "DOCTOR", Description = "Medical Doctor" };
            var patientRole = new Role { Name = "Patient", NormalizedName = "PATIENT", Description = "Patient" };
            var staffRole = new Role { Name = "Staff", NormalizedName = "STAFF", Description = "Clinic Staff" };

            await context.Roles.AddRangeAsync(adminRole, doctorRole, patientRole, staffRole);
            await context.SaveChangesAsync();
        }

        // 2. Admin User
        if (!await context.Users.AnyAsync(u => u.NormalizedEmail == "ADMIN@MEDICORE.LOCAL"))
        {
            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@medicore.local",
                NormalizedEmail = "ADMIN@MEDICORE.LOCAL",
                EmailConfirmed = true,
                PhoneNumber = "+10000000000",
                PhoneNumberConfirmed = true,
                PasswordHash = "hashed_password_here", // Should be hashed properly in real app
                IsActive = true
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "SYSTEMADMIN");
            if (adminRole != null)
            {
                await context.UserRoles.AddAsync(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }

        // 3. Specializations
        if (!await context.Specializations.AnyAsync())
        {
            var specializations = new List<Specialization>
            {
                new Specialization { Name = "General Practice", Description = "Primary care for general health issues" },
                new Specialization { Name = "Cardiology", Description = "Heart and cardiovascular system" },
                new Specialization { Name = "Dermatology", Description = "Skin, hair, and nails" },
                new Specialization { Name = "Neurology", Description = "Nervous system disorders" }
            };

            await context.Specializations.AddRangeAsync(specializations);
            await context.SaveChangesAsync();
        }

        // 4. Cities (Sample)
        if (!await context.Cities.AnyAsync())
        {
            var cities = new List<City>
            {
                new City { Name = "New York", StateOrProvince = "NY", Country = "USA" },
                new City { Name = "London", StateOrProvince = "ENG", Country = "UK" },
                new City { Name = "Toronto", StateOrProvince = "ON", Country = "Canada" }
            };

            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();
        }
    }
}
