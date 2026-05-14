using System.Text.Json;
using FluentValidation;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            await WriteProblemAsync(context, StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.", errors);
        }
        catch (UnauthorizedException)
        {
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, "Invalid credentials.");
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            var detail = context.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsDevelopment()
                ? ex.ToString()
                : null;

            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.", detail: detail);
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int status,
        string title,
        IDictionary<string, string[]>? errors = null,
        string? detail = null)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new Dictionary<string, object?>
        {
            ["status"] = status,
            ["title"] = title,
        };

        if (detail is not null)
            problem["detail"] = detail;

        if (errors is { Count: > 0 })
            problem["errors"] = errors;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }));
    }
}
