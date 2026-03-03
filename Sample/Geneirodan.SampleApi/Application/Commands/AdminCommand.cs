using Ardalis.Result;
using Geneirodan.MediatR.Abstractions;
using Geneirodan.MediatR.Attributes;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command protected by <see cref="AuthorizeAttribute"/> with <c>Roles = "Admin"</c>: only users in the Admin role can execute it.
/// Demonstrates role-based authorization in the MediatR pipeline; returns Forbidden if the user is not in the Admin role.
/// When authorized, returns success or <see cref="Result.Error"/> when <see cref="ShouldFail"/> is <see langword="true"/>.
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, returns <see cref="Result.Error"/> instead of success.</param>
[Authorize(Roles = "Admin")]
public sealed record AdminCommand(bool ShouldFail) : ICommand
{
    /// <summary>
    /// Handles the command after role authorization has passed.
    /// </summary>
    public sealed class Handler : IRequestHandler<AdminCommand, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(AdminCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? Result.Error("SomeSortOfError") : Result.Success());
    }
}