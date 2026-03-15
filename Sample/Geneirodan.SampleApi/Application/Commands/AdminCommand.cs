using Ardalis.Result;
using Geneirodan.MediatR.Abstractions;
using Geneirodan.MediatR.Attributes;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command protected by <see cref="AuthorizeAttribute"/> with <c>Roles = "Admin"</c>: only users in the Admin role can execute it.
/// Demonstrates role-based authorization in the MediatR pipeline; returns Forbidden if the user is not in the Admin role.
/// When authorized, returns success or error result depending on <see cref="ShouldFail"/> value.
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, returns error instead of success.</param>
[Authorize(Roles = "Admin")]
public sealed record AdminCommand(bool ShouldFail) : ICommand
{
    /// <inheritdoc/>
    public sealed class Handler : IRequestHandler<AdminCommand, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(AdminCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? Result.Error("SomeSortOfError") : Result.Success());
    }
}