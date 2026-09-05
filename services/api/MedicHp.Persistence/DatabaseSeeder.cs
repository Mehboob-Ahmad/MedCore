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

        // 4. Pakistani Cities — Additive seeding (inserts only cities not already present)
        await SeedPakistaniCitiesAsync(context);
    }

    private static async Task SeedPakistaniCitiesAsync(ApplicationDbContext context)
    {
        var allCities = new List<(string Name, string Province)>
        {
            // ── Punjab ──
            ("Lahore", "Punjab"),
            ("Faisalabad", "Punjab"),
            ("Rawalpindi", "Punjab"),
            ("Multan", "Punjab"),
            ("Gujranwala", "Punjab"),
            ("Sialkot", "Punjab"),
            ("Bahawalpur", "Punjab"),
            ("Sargodha", "Punjab"),
            ("Sahiwal", "Punjab"),
            ("Okara", "Punjab"),
            ("Jhang", "Punjab"),
            ("Sheikhupura", "Punjab"),
            ("Rahim Yar Khan", "Punjab"),
            ("Dera Ghazi Khan", "Punjab"),
            ("Gujrat", "Punjab"),
            ("Kasur", "Punjab"),
            ("Mianwali", "Punjab"),
            ("Attock", "Punjab"),
            ("Chakwal", "Punjab"),
            ("Jhelum", "Punjab"),
            ("Hafizabad", "Punjab"),
            ("Mandi Bahauddin", "Punjab"),
            ("Chiniot", "Punjab"),
            ("Toba Tek Singh", "Punjab"),
            ("Khanewal", "Punjab"),
            ("Vehari", "Punjab"),
            ("Lodhran", "Punjab"),
            ("Muzaffargarh", "Punjab"),
            ("Layyah", "Punjab"),
            ("Bhakkar", "Punjab"),
            ("Pakpattan", "Punjab"),
            ("Narowal", "Punjab"),
            ("Khushab", "Punjab"),
            ("Nankana Sahib", "Punjab"),
            ("Rajanpur", "Punjab"),
            ("Bahawalnagar", "Punjab"),
            ("Murree", "Punjab"),
            ("Taxila", "Punjab"),
            ("Wah Cantt", "Punjab"),
            ("Kamoke", "Punjab"),
            ("Burewala", "Punjab"),
            ("Chishtian", "Punjab"),
            ("Haroonabad", "Punjab"),
            ("Kot Addu", "Punjab"),
            ("Ahmadpur East", "Punjab"),
            ("Jaranwala", "Punjab"),
            ("Samundri", "Punjab"),
            ("Talagang", "Punjab"),
            ("Daska", "Punjab"),
            ("Wazirabad", "Punjab"),

            // ── Sindh ──
            ("Karachi", "Sindh"),
            ("Hyderabad", "Sindh"),
            ("Sukkur", "Sindh"),
            ("Larkana", "Sindh"),
            ("Nawabshah", "Sindh"),
            ("Mirpur Khas", "Sindh"),
            ("Thatta", "Sindh"),
            ("Jacobabad", "Sindh"),
            ("Shikarpur", "Sindh"),
            ("Khairpur", "Sindh"),
            ("Dadu", "Sindh"),
            ("Badin", "Sindh"),
            ("Tando Allahyar", "Sindh"),
            ("Tando Adam", "Sindh"),
            ("Umerkot", "Sindh"),
            ("Sanghar", "Sindh"),
            ("Ghotki", "Sindh"),
            ("Kashmore", "Sindh"),
            ("Matiari", "Sindh"),
            ("Jamshoro", "Sindh"),
            ("Tharparkar", "Sindh"),
            ("Shahdadkot", "Sindh"),
            ("Kambar", "Sindh"),
            ("Kandhkot", "Sindh"),
            ("Hala", "Sindh"),
            ("Sehwan", "Sindh"),

            // ── Khyber Pakhtunkhwa ──
            ("Peshawar", "Khyber Pakhtunkhwa"),
            ("Mardan", "Khyber Pakhtunkhwa"),
            ("Abbottabad", "Khyber Pakhtunkhwa"),
            ("Mingora", "Khyber Pakhtunkhwa"),
            ("Kohat", "Khyber Pakhtunkhwa"),
            ("Dera Ismail Khan", "Khyber Pakhtunkhwa"),
            ("Swat", "Khyber Pakhtunkhwa"),
            ("Nowshera", "Khyber Pakhtunkhwa"),
            ("Charsadda", "Khyber Pakhtunkhwa"),
            ("Bannu", "Khyber Pakhtunkhwa"),
            ("Mansehra", "Khyber Pakhtunkhwa"),
            ("Swabi", "Khyber Pakhtunkhwa"),
            ("Haripur", "Khyber Pakhtunkhwa"),
            ("Lakki Marwat", "Khyber Pakhtunkhwa"),
            ("Tank", "Khyber Pakhtunkhwa"),
            ("Batagram", "Khyber Pakhtunkhwa"),
            ("Buner", "Khyber Pakhtunkhwa"),
            ("Shangla", "Khyber Pakhtunkhwa"),
            ("Lower Dir", "Khyber Pakhtunkhwa"),
            ("Upper Dir", "Khyber Pakhtunkhwa"),
            ("Chitral", "Khyber Pakhtunkhwa"),
            ("Karak", "Khyber Pakhtunkhwa"),
            ("Hangu", "Khyber Pakhtunkhwa"),
            ("Timergara", "Khyber Pakhtunkhwa"),
            ("Daggar", "Khyber Pakhtunkhwa"),

            // ── Balochistan ──
            ("Quetta", "Balochistan"),
            ("Gwadar", "Balochistan"),
            ("Turbat", "Balochistan"),
            ("Khuzdar", "Balochistan"),
            ("Chaman", "Balochistan"),
            ("Sibi", "Balochistan"),
            ("Zhob", "Balochistan"),
            ("Loralai", "Balochistan"),
            ("Hub", "Balochistan"),
            ("Dera Murad Jamali", "Balochistan"),
            ("Pishin", "Balochistan"),
            ("Mastung", "Balochistan"),
            ("Kalat", "Balochistan"),
            ("Nushki", "Balochistan"),
            ("Panjgur", "Balochistan"),
            ("Dera Bugti", "Balochistan"),
            ("Usta Muhammad", "Balochistan"),
            ("Bela", "Balochistan"),
            ("Dalbandin", "Balochistan"),
            ("Ziarat", "Balochistan"),
            ("Kharan", "Balochistan"),
            ("Washuk", "Balochistan"),
            ("Awaran", "Balochistan"),
            ("Jiwani", "Balochistan"),
            ("Surab", "Balochistan"),

            // ── Islamabad Capital Territory ──
            ("Islamabad", "Islamabad Capital Territory"),

            // ── Azad Jammu & Kashmir ──
            ("Muzaffarabad", "Azad Jammu & Kashmir"),
            ("Mirpur", "Azad Jammu & Kashmir"),
            ("Rawalakot", "Azad Jammu & Kashmir"),
            ("Bhimber", "Azad Jammu & Kashmir"),
            ("Kotli", "Azad Jammu & Kashmir"),
            ("Bagh", "Azad Jammu & Kashmir"),
            ("Pallandri", "Azad Jammu & Kashmir"),
            ("Athmuqam", "Azad Jammu & Kashmir"),
            ("Hattian Bala", "Azad Jammu & Kashmir"),
            ("Haveli", "Azad Jammu & Kashmir"),

            // ── Gilgit-Baltistan ──
            ("Gilgit", "Gilgit-Baltistan"),
            ("Skardu", "Gilgit-Baltistan"),
            ("Hunza", "Gilgit-Baltistan"),
            ("Chilas", "Gilgit-Baltistan"),
            ("Ghanche", "Gilgit-Baltistan"),
            ("Astore", "Gilgit-Baltistan"),
            ("Khaplu", "Gilgit-Baltistan"),
            ("Gahkuch", "Gilgit-Baltistan"),
            ("Aliabad", "Gilgit-Baltistan"),
            ("Nagar", "Gilgit-Baltistan"),
        };

        // Fetch existing city names to avoid duplicates
        var existingCityNames = await context.Cities
            .Select(c => c.Name.ToUpper())
            .ToListAsync();

        var existingSet = new HashSet<string>(existingCityNames);
        var citiesToAdd = new List<City>();

        foreach (var (name, province) in allCities)
        {
            if (!existingSet.Contains(name.ToUpper()))
            {
                citiesToAdd.Add(new City
                {
                    Name = name,
                    StateOrProvince = province,
                    Country = "Pakistan",
                    IsActive = true
                });
            }
        }

        if (citiesToAdd.Count > 0)
        {
            await context.Cities.AddRangeAsync(citiesToAdd);
            await context.SaveChangesAsync();
        }
    }
}
