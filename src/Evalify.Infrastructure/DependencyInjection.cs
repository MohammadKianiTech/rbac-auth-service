using System.Text;
using Asp.Versioning;
using Evalify.Application.Abstractions.Authentication;
using Evalify.Application.Abstractions.Caching;
using Evalify.Application.Abstractions.Clock;
using Evalify.Application.Abstractions.Data;
using Evalify.Application.Abstractions.Hashing;
using Evalify.Domain.Abstractions;
using Evalify.Domain.Repositories;
using Evalify.Infrastructure.Authentication;
using Evalify.Infrastructure.Authorization;
using Evalify.Infrastructure.Caching;
using Evalify.Infrastructure.Clock;
using Evalify.Infrastructure.Data;
using Evalify.Infrastructure.Hashing;
using Evalify.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Evalify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        AddPersistenc(services, configuration);
        AddCaching(services, configuration);
        AddHealthCheck(services, configuration);
        AddApiVersioning(services);
        AddAuthentication(services, configuration);
        AddAuthorization(services);
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }


    private static void AddPersistenc(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ?? throw new ArgumentException("Connection string 'Database' is not configured in appsettings.json.", nameof(configuration));
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
    }
    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cache") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();
    }
    private static void AddHealthCheck(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Cache")!);
    }
    private static void AddApiVersioning(IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
    }
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options => configuration.GetSection("JwtOptions").Bind(options));
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITokenValidatorService, TokenValidatorService>();
        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = true;
            cfg.SaveToken = true;
            var bearerTokenOption = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            if (bearerTokenOption is null)
            {
                throw new InvalidOperationException("bearerTokenOption is null");
            }
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                // site that makes the token
                ValidIssuer = bearerTokenOption.Issuer,
                // site that consumes the token
                ValidAudience = bearerTokenOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearerTokenOption.Key)),
                // verify signature to avoid tampering
                ValidateIssuerSigningKey = true,
                // validate the expiration
                ValidateLifetime = true,
                // tolerance for the expiration date
                ClockSkew = TimeSpan.Zero,
            };
            cfg.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    logger.LogError(context.Exception, "Authentication failed");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                    return tokenValidatorService.ValidateAsync(context);
                },
                OnMessageReceived = context =>
                {
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                    logger.LogError("OnChallenge error: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
    }
    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

}
