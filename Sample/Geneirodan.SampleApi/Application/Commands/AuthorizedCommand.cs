using Ardalis.Result;
using Geneirodan.MediatR.Abstractions;
using Geneirodan.MediatR.Attributes;
using Geneirodan.MediatR.Behaviors;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command protected by <see cref="AuthorizeAttribute"/> (no roles): requires an authenticated user.
/// Demonstrates <see cref="AuthorizationBehavior{TRequest, TResponse}"/>; returns Unauthorized if <see cref="IUser.Id"/> is empty.
/// When authorized, returns success or error result depending on <see cref="ShouldFail"/> value.
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, returns error instead of success.</param>
[Authorize]
public sealed record AuthorizedCommand(bool ShouldFail) : ICommand
{
    /// <inheritdoc/>
    public sealed class Handler : IRequestHandler<AuthorizedCommand, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(AuthorizedCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? Result.Error("SomeSortOfError") : Result.Success());
    }
}