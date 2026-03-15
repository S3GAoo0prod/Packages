using Ardalis.Result;
using FluentValidation;
using Geneirodan.MediatR.Abstractions;
using Geneirodan.MediatR.Behaviors;
using JetBrains.Annotations;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command that either succeeds or throws an exception when <see cref="ShouldFail"/> is <see langword="true"/>.
/// Used to demonstrate the MediatR pipeline and exception handling (e.g. <see cref="UnhandledExceptionBehavior{TRequest, TResponse}"/> and ASP.NET Core exception handler).
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, the handler throws; otherwise returns success.</param>
public sealed record Command(bool ShouldFail) : ICommand
{
    /// <inheritdoc/>
    public sealed class Handler : IRequestHandler<Command, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(Command request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? throw new Exception("SomeSortOfError") : Result.Success());
    }
}

/// <summary>
/// Sample command that carries an email and is validated by FluentValidation before the handler runs.
/// Demonstrates <see cref="ValidationBehavior{TRequest, TResponse}"/> and <see cref="AbstractValidator{T}"/>.
/// </summary>
/// <param name="Email">The email to validate; must be non-empty and a valid email address format.</param>
public sealed record ValidatedCommand(string Email) : ICommand
{
    /// <inheritdoc/>
    public sealed class Handler : IRequestHandler<ValidatedCommand, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(ValidatedCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success());
    }

    /// <inheritdoc/>
    [UsedImplicitly]
    public sealed class Validator : AbstractValidator<ValidatedCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}