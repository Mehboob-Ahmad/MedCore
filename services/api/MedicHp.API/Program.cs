using MedicHp.API.Middlewares;
using MedicHp.Application;
using MedicHp.Infrastructure;
using MedicHp.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;

// Load .env file
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Ensure DATABASE_URL from .env overrides DefaultConnection
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(dbUrl))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = dbUrl;
}

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add Dependency Injection Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

// Add API Configuration
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Performance Optimization
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Security: Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<Microsoft.AspNetCore.Http.HttpContext, string>(httpContext =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? string.Empty))
    };
    
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userIdStr = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<MedicHp.Persistence.ApplicationDbContext>();
                var user = await dbContext.Users.FindAsync(userId);
                if (user == null || !user.IsActive)
                {
                    context.Fail("Your account has been frozen. Please contact MedicHp administration.");
                }
            }
        }
    };
});

// Rate Limiting & CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy => 
        policy.SetIsOriginAllowed(origin => 
                  origin.EndsWith(".medichp.com") || 
                  origin == "https://medichp.com" ||
                  origin.EndsWith(".medichp.co") || 
                  origin == "https://medichp.co" ||
                  origin.EndsWith(".vercel.app") ||
                  origin.StartsWith("http://localhost:"))
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
              
    options.AddPolicy("AllowAll", policy => 
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MedicHp.Persistence.ApplicationDbContext>();
    
    // Apply pending migrations automatically
    await context.Database.MigrateAsync();
    
    // Seed data
    await MedicHp.Persistence.DatabaseSeeder.SeedAsync(context);
}

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}
else
{
    app.UseHsts();
    app.UseCors("ProductionCors");
}

app.UseResponseCompression();
app.UseRateLimiter();

// Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
    await next();
});

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<MedicHp.API.Hubs.ChatHub>("/hubs/chat");

app.Run();
