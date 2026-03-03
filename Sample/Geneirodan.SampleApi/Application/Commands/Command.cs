using Ardalis.Result;
using FluentValidation;
using Geneirodan.MediatR.Abstractions;
using JetBrains.Annotations;
using MediatR;

namespace Geneirodan.SampleApi.Application.Commands;

/// <summary>
/// Sample command that either succeeds or throws an exception when <see cref="ShouldFail"/> is <see langword="true"/>.
/// Used to demonstrate the MediatR pipeline and exception handling (e.g. <see cref="Geneirodan.MediatR.Behaviors.UnhandledExceptionBehavior{TRequest, TResponse}"/> and ASP.NET Core exception handler).
/// </summary>
/// <param name="ShouldFail">When <see langword="true"/>, the handler throws; otherwise returns <see cref="Result.Success"/>.</param>
public sealed record Command(bool ShouldFail) : ICommand
{
    /// <summary>
    /// Handles the command: returns success or throws so that the pipeline can log and the API can return problem details.
    /// </summary>
    public sealed class Handler : IRequestHandler<Command, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(Command request, CancellationToken cancellationToken) =>
            Task.FromResult(request.ShouldFail ? throw new Exception("SomeSortOfError") : Result.Success());
    }
}

/// <summary>
/// Sample command that carries an email and is validated by FluentValidation before the handler runs.
/// Demonstrates <see cref="Geneirodan.MediatR.Behaviors.ValidationBehavior{TRequest, TResponse}"/> and <see cref="AbstractValidator{T}"/>.
/// </summary>
/// <param name="Email">The email to validate; must be non-empty and a valid email address format.</param>
public sealed record ValidatedCommand(string Email) : ICommand
{
    /// <summary>
    /// Handles the command after validation has passed; returns success.
    /// </summary>
    public sealed class Handler : IRequestHandler<ValidatedCommand, Result>
    {
        /// <inheritdoc/>
        public Task<Result> Handle(ValidatedCommand request, CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success());
    }

    /// <summary>
    /// FluentValidation validator for <see cref="ValidatedCommand"/>. Runs in the pipeline before the handler.
    /// </summary>
    [UsedImplicitly]
    public sealed class Validator : AbstractValidator<ValidatedCommand>
    {
        /// <summary>
        /// Configures rules: <see cref="ValidatedCommand.Email"/> must be non-empty and a valid email address.
        /// </summary>
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}