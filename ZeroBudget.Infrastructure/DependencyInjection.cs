using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Infrastructure.Persistence;
using ZeroBudget.Infrastructure.Persistence.Repositories;
using ZeroBudget.Infrastructure.Security;

namespace ZeroBudget.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Security ──────────────────────────────────────────────────────────
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        var email = configuration["Auth:Email"]
            ?? throw new InvalidOperationException("Auth:Email is required.");
        var password = configuration["Auth:Password"]
            ?? throw new InvalidOperationException("Auth:Password is required.");
        services.AddSingleton<IAuthSettings>(new AuthSettings(email, password));

        var jwtOpts = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                      ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(bearer =>
            {
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOpts.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOpts.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOpts.Secret)),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }
}
