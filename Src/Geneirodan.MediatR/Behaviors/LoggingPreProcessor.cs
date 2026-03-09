using Geneirodan.Abstractions.Domain;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Geneirodan.MediatR.Behaviors;

/// <summary>
/// A pipeline behavior that logs information about incoming requests before they are processed.
/// It logs the name of the request, the user ID, and the details of the request itself.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being processed. This should be a non-nullable type.
/// </typeparam>
public sealed partial class LoggingPreProcessor<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly IUser? _user;

    /// <inheritdoc/>
    public LoggingPreProcessor(ILogger<TRequest> logger, IUser? user) : this(logger) => _user = user;

    /// <inheritdoc/>
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