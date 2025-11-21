using Microsoft.AspNetCore.Mvc;

namespace HabitFlow.Infrastructure.Exceptions;

/// <summary>
/// Classe para centralizar tratamento de erros e geração de ProblemDetails
/// </summary>
public static class ErrorHandler
{
    public static ProblemDetails CreateProblemDetails(
        string title,
        string detail,
        int statusCode,
        string? type = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Type = type ?? $"https://httpstatuses.com/{statusCode}"
        };
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        string detail,
        Dictionary<string, string[]> errors)
    {
        return new ValidationProblemDetails
        {
            Title = "Erro de Validação",
            Detail = detail,
            Status = 400,
            Type = "https://httpstatuses.com/400"
        };
    }
}

/// <summary>
/// Middleware para tratamento global de exceções
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Uma exceção não tratada ocorreu");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var response = exception switch
        {
            InvalidOperationException => new
            {
                status = 400,
                title = "Operação Inválida",
                detail = exception.Message,
                type = "https://httpstatuses.com/400"
            },
            KeyNotFoundException => new
            {
                status = 404,
                title = "Recurso Não Encontrado",
                detail = exception.Message,
                type = "https://httpstatuses.com/404"
            },
            ArgumentException => new
            {
                status = 400,
                title = "Argumento Inválido",
                detail = exception.Message,
                type = "https://httpstatuses.com/400"
            },
            _ => new
            {
                status = 500,
                title = "Erro Interno do Servidor",
                detail = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.",
                type = "https://httpstatuses.com/500"
            }
        };

        context.Response.StatusCode = (int)response.GetType().GetProperty("status")!.GetValue(response)!;
        return context.Response.WriteAsJsonAsync(response);
    }
}
