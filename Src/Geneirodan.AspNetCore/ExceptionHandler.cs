using JetBrains.Annotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Central exception handler for the pipeline; converts unhandled exceptions into HTTP status codes and RFC 7807 problem details.
/// Registered when <see cref="DependencyInjection.AddErrorHandling"/> is used. Maps exception types to status codes:
/// <see cref="InvalidOperationException"/> and <see cref="ArgumentException"/> to 422 Unprocessable Entity;
/// <see cref="BadHttpRequestException"/> and <see cref="FormatException"/> to 400 Bad Request; all others to 500 Internal Server Error.
/// Writes the response via <see cref="IProblemDetailsService"/> so that middleware and filters get a consistent error shape.
/// </summary>
[PublicAPI]
public class ExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle the exception by setting the response status code and writing problem details.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="exception">The unhandled exception that occurred.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the exception was handled and the response was written; otherwise <see langword="false"/>.</returns>
    public virtual async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            InvalidOperationException or ArgumentException => StatusCodes.Status422UnprocessableEntity,
            BadHttpRequestException or FormatException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        var context = new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Detail = exception.Message
            }
        };
        return await problemDetailsService.TryWriteAsync(context).ConfigureAwait(false);
    }
}