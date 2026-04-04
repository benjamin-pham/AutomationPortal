using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutomationPortal.Application.Abstractions.Authentication;
using AutomationPortal.Application.Abstractions.Data;
using AutomationPortal.Domain.Abstractions;
using AutomationPortal.Domain.Repositories;
using AutomationPortal.Infrastructure.Authentication;
using AutomationPortal.Infrastructure.Caching;
using AutomationPortal.Infrastructure.Clock;
using AutomationPortal.Infrastructure.Data;
using AutomationPortal.Infrastructure.Repositories;

namespace AutomationPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddSingleton<ISqlConnectionFactory>(
            _ => new SqlConnectionFactory(connectionString));

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddDistributedMemoryCache();
        services.AddScoped<ICacheService, CacheService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGeminiKeyRepository, GeminiKeyRepository>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
