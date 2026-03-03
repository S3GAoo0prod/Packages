using Geneirodan.Abstractions.Domain;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Geneirodan.MediatR.Behaviors;

/// <summary>
/// MediatR request pre-processor that logs each incoming request before the pipeline runs.
/// Logs the request type name and, when <see cref="IUser"/> is available and authenticated, the user ID.
/// Request payload is pushed to Serilog's log context so that structured logging can capture it.
/// </summary>
/// <typeparam name="TRequest">The type of the request (command or query) being processed.</typeparam>
public sealed partial class LoggingPreProcessor<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly IUser? _user;

    /// <summary>
    /// Constructor used when <see cref="IUser"/> is registered; allows logging the current user ID.
    /// </summary>
    public LoggingPreProcessor(ILogger<TRequest> logger, IUser? user) : this(logger) => _user = user;

    /// <summary>
    /// Logs the request name and optional user ID, then completes so that the pipeline continues.
    /// </summary>
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        using (LogContext.PushProperty("Request", request, destructureObjects: true))
            if (_user is not null && _user.Id != Guid.Empty)
                LogProcessingRequestWithUserId(requestName, _user.Id);
            else
                LogProcessingRequest(requestName);

        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Processing {RequestName}")]
    partial void LogProcessingRequest(string requestName);

    [LoggerMessage(LogLevel.Information, "Processing {RequestName} with user {UserId}")]
    partial void LogProcessingRequestWithUserId(string requestName, Guid userId);
}