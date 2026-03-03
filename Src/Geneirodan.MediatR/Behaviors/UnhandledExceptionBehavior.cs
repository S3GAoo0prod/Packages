using Ardalis.Result;
using Microsoft.Extensions.Logging;

namespace Geneirodan.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that catches any exception thrown by the handler (or inner behaviors), logs it with the request name,
/// then rethrows so that the ASP.NET Core exception handler (or host) can convert it to a response. Does not convert
/// exceptions to <see cref="Result"/>; use handler try/catch or validation/authorization for that.
/// </summary>
/// <typeparam name="TRequest">The type of the request being processed.</typeparam>
/// <typeparam name="TResponse">The type of the response (e.g. <see cref="Result"/> or <see cref="Result{T}"/>).</typeparam>
public sealed partial class UnhandledExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Invokes the next delegate; on exception, logs and rethrows.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogError(ex, typeof(TRequest).Name);
            throw;
        }
    }
    
    [LoggerMessage(LogLevel.Error, "Request: Unhandled Exception for Request {RequestName}")]
    partial void LogError(Exception ex, string requestName);
}
