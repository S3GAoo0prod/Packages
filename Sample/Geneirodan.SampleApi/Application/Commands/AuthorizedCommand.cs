using Ardalis.Result;
using Geneirodan.MediatR.Abstractions;
using Geneirodan.MediatR.Attributes;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command protected by <see cref="AuthorizeAttribute"/> (no roles): requires an authenticated user.
/// Demonstrates <see cref="Geneirodan.MediatR.Behaviors.AuthorizationBehavior{TRequest, TResponse}"/>; returns Unauthorized if <see cref="IUser.Id"/> is empty.
/// When authorized, returns success or <see cref="Result.Error"/> when <see cref="ShouldFail"/> is <see langword="true"/>.
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, returns <see cref="Result.Error"/> instead of success.</param>
[Authorize]
public sealed record AuthorizedCommand(bool ShouldFail) : ICommand
{
    /// <summary>
    /// Handles the command after authorization has passed.
    /// </summary>
    public sealed class Handler : IRequestHandler<AuthorizedCommand, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(AuthorizedCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? Result.Error("SomeSortOfError") : Result.Success());
    }
}