using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ZeroBudget.Application.Common;

namespace ZeroBudget.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers MediatR handlers, validation pipeline, and FluentValidation validators.
    /// Call services.Configure&lt;BudgetOptions&gt;(...) in the API layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
