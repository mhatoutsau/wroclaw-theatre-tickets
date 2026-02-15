namespace WroclawTheatreTickets.Web;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MediatR;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.Contracts.Cache;
using WroclawTheatreTickets.Application.Mapping;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;
using WroclawTheatreTickets.Infrastructure.Configuration;
using WroclawTheatreTickets.Infrastructure.Data;
using WroclawTheatreTickets.Infrastructure.Repositories;
using WroclawTheatreTickets.Infrastructure.Services;
using WroclawTheatreTickets.Infrastructure.Cache;
using FluentValidation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<TheatreDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=theatre.db"));

        // MediatR - scan Application assembly for handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SaveOrUpdateShowCommand>());

        // Cache - IDistributedCache with in-memory backend (can be replaced with Redis)
        services.AddDistributedMemoryCache();
        services.AddSingleton<CacheMetrics>();
        services.AddSingleton<ICacheService, CacheService>();
        services.Configure<CacheOptions>(configuration.GetSection(CacheKeys.ConfigurationSection));

        // AutoMapper
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // Repositories
        services.AddScoped<ITheatreRepository, TheatreRepository>();
        services.AddScoped<IShowRepository, ShowRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IViewHistoryRepository, ViewHistoryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Theatre Repertoire Sync Services
        services.Configure<TheatreApiConfiguration>(configuration.GetSection("TheatreApis:TeatrPolski"));
        services.AddScoped<IRepertoireDataService, TeatrPolskiRepertoireDataService>();
        services.AddScoped<ITheatreProviderService, TheatreProviderService>();
        services.AddScoped<ITheatreRepertoireSyncService, TheatreRepertoireSyncService>();

        // FluentValidation - scan Application assembly for validators
        services.AddValidatorsFromAssemblyContaining<SaveOrUpdateShowCommand>();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Authentication
        var jwtSecret = configuration["Jwt:Secret"] ?? throw new Exception("JWT Secret not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "WroclawTheatreTickets",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "WroclawTheatreTicketsUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ModeratorOrAdmin", policy => policy.RequireRole("Moderator", "Admin"));
        });

        // Rate Limiting
        services.AddRateLimitingPolicies();

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Wrocław Theatre Tickets API",
                Version = "v1",
                Description = "API for managing theatre shows, users, favorites, and reviews in Wrocław",
                Contact = new OpenApiContact
                {
                    Name = "Wrocław Theatre Tickets",
                    Email = "support@example.com"
                }
            });

            // Add JWT Bearer authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
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
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
