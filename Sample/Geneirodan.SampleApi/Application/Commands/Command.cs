using Ardalis.Result;
using FluentValidation;
using Geneirodan.MediatR.Abstractions;
using JetBrains.Annotations;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command used by tests and documentation to exercise the MediatR pipeline and exception handling.
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, the handler throws; otherwise returns <see cref="Result.Success"/>.</param>
public sealed record Command(bool ShouldFail) : ICommand
{
    /// <inheritdoc/>
    public sealed class Handler : IRequestHandler<Command, Result>
    {
        public Task<Result> Handle(Command request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? throw new Exception("SomeSortOfError") : Result.Success());
    }
}

/// <summary>
/// Sample command that demonstrates FluentValidation before the handler runs.
/// </summary>
/// <param name="Email">The email to validate; must be non-empty and a valid email address format.</param>
public sealed record ValidatedCommand(string Email) : ICommand
{
    /// <inheritdoc/>
    public sealed class Handler : IRequestHandler<ValidatedCommand, Result>
    {
        public Task<Result> Handle(ValidatedCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success());
    }

    [UsedImplicitly]
    public sealed class Validator : AbstractValidator<ValidatedCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}