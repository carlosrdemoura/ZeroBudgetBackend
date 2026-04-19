namespace ZeroBudget.Api.Extensions;

public static class CustomCorsExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedDomains = configuration["CorsAllowedDomains"]?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrEmpty(origin))
                            return false;

                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            return false;

                        return allowedDomains.Any(domain =>
                            uri.Host.Equals(domain, StringComparison.OrdinalIgnoreCase) ||
                            uri.Host.EndsWith($".{domain}", StringComparison.OrdinalIgnoreCase));
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors();

        return app;
    }
}