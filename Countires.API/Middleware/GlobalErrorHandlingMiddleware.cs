using Countries.Application.DTOs.Shared;
using Countries.Application.Exceptions;
using FluentValidation;

namespace Countries.API.Middleware
{
    public sealed class ErrorHandlingMiddleware(
       ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException)
                when (context.RequestAborted.IsCancellationRequested)
            {
                // The client disconnected or cancelled the request so no need for server to continue the request
            }
            catch (Exception exception) when (!context.Response.HasStarted)
            {
                var statusCode = StatusCodes.Status500InternalServerError;
                var message = "An unexpected error occurred. Please try again later.";

                Dictionary<string, string[]>? errors = null;

                switch (exception)
                {
                    case ValidationException validationException:
                        statusCode = StatusCodes.Status400BadRequest;
                        message = "Validation failed.";

                        errors = validationException.Errors
                            .GroupBy(error =>
                                string.IsNullOrWhiteSpace(error.PropertyName)
                                    ? "Request"
                                    : error.PropertyName)
                            .ToDictionary(
                                group => group.Key,
                                group => group
                                    .Select(error => error.ErrorMessage)
                                    .Distinct()
                                    .ToArray());
                        break;

                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        message = exception.Message;
                        break;

                    case ConflictException:
                        statusCode = StatusCodes.Status409Conflict;
                        message = exception.Message;
                        break;
                }

                if (statusCode == StatusCodes.Status500InternalServerError)
                {
                    logger.LogError(
                        exception,
                        "Unhandled error for {Method} {Path}",
                        context.Request.Method,
                        context.Request.Path);
                }
                else
                {
                    logger.LogWarning(
                        "Request failed with status {StatusCode}: {Message}",
                        statusCode,
                        message);
                }

                context.Response.Clear();
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object?>.Fail(message, errors),
                    cancellationToken: context.RequestAborted);
            }
        }
    }
}
