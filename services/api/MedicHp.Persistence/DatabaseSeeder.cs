using MedicHp.Domain.Entities.Core;
using MedicHp.Domain.Entities.Lookup;
using Microsoft.EntityFrameworkCore;

namespace MedicHp.Persistence;

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
        if (!await context.Users.AnyAsync(u => u.NormalizedEmail == "MEHBOOBAHMAD122005@GMAIL.COM"))
        {
            var adminUser = new User
            {
                FirstName = "Mehboob",
                LastName = "Ahmad",
                Email = "mehboobahmad122005@gmail.com",
                NormalizedEmail = "MEHBOOBAHMAD122005@GMAIL.COM",
                EmailConfirmed = true,
                PhoneNumber = "+10000000000",
                PhoneNumberConfirmed = true,
                IsActive = true
            };

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

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
                new Specialization { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "General Practice", Description = "Primary care for general health issues" },
                new Specialization { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Cardiology", Description = "Heart and cardiovascular system" },
                new Specialization { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Dermatology", Description = "Skin, hair, and nails" },
                new Specialization { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Neurology", Description = "Nervous system disorders" }
            };

            await context.Specializations.AddRangeAsync(specializations);
            await context.SaveChangesAsync();
        }

        // 4. Cities (Sample)
        if (!await context.Cities.AnyAsync())
        {
            var cities = new List<City>
            {
                new City { Name = "Lahore", StateOrProvince = "Punjab", Country = "Pakistan" },
                new City { Name = "Karachi", StateOrProvince = "Sindh", Country = "Pakistan" },
                new City { Name = "Islamabad", StateOrProvince = "ICT", Country = "Pakistan" },
                new City { Name = "Faisalabad", StateOrProvince = "Punjab", Country = "Pakistan" },
                new City { Name = "Rawalpindi", StateOrProvince = "Punjab", Country = "Pakistan" },
                new City { Name = "Multan", StateOrProvince = "Punjab", Country = "Pakistan" },
                new City { Name = "Sahiwal", StateOrProvince = "Punjab", Country = "Pakistan" }
            };

            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();
        }
    }
}
