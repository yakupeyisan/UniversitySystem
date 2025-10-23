
using API.Middlewares;
using Core.Application.Extensions;
using Core.Infrastructure.Extensions;
using Core.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using PersonMgmt.Application.Extensions;
using Serilog;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURATION ====================

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ==================== LOGGING ====================

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting application...");

    // ==================== SERVICES ====================

    // Add Controllers
    builder.Services.AddControllers();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // ==================== DEPENDENCY INJECTION ====================

    // Core layers
    builder.Services.AddCoreApplication();
    //builder.Services.AddCoreInfrastructure();

    // PersonMgmt module
    builder.Services.AddPersonMgmtApplication();

    // Infrastructure (centralized for all modules)
    builder.Services.AddInfrastructure(builder.Configuration);

    // ==================== SWAGGER/OPENAPI ====================

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "University System API",
            Version = "v1",
            Description = "API documentation for University System - PersonMgmt Module",
            Contact = new OpenApiContact
            {
                Name = "Development Team",
                Url = new Uri("https://github.com/yourrepo")
            }
        });

        // Add JWT Authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });

        // Include XML comments if available
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // ==================== HEALTH CHECKS ====================

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>(
            name: "database",
            tags: new[] { "db", "sql" });

    // ==================== BUILD ====================

    var app = builder.Build();

    // ==================== MIDDLEWARE ====================

    // Global exception handler (must be first)
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // HTTPS redirection
    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    // CORS
    app.UseCors("AllowAll");

    // Routing
    app.UseRouting();

    // Logging middleware
    app.UseMiddleware<RequestLoggingMiddleware>();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // ==================== SWAGGER UI ====================

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "University System API v1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at root
        });
    }

    // ==================== ROUTES ====================

    app.MapControllers();

    // Health check endpoint
    app.MapHealthChecks("/health");

    // ==================== DATABASE MIGRATION ====================

    // Auto-migrate on startup (development only)
    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
                Log.Information("Database migration completed successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during database migration");
                throw;
            }
        }
    }

    // ==================== START ====================

    Log.Information("Application started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

