using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;

namespace Geneirodan.MediatR.Behaviors;

/// <summary>
/// Pipeline behavior that runs all FluentValidation validators registered for <typeparamref name="TRequest"/> before the handler.
/// If any validator fails, returns an <see cref="Ardalis.Result.ResultStatus.Invalid"/> result with the validation errors
/// via <see cref="DynamicResults.Invalid{TResponse}"/>; otherwise calls the next delegate. Validators are resolved from the container.
/// </summary>
/// <typeparam name="TRequest">The type of the request (command or query) to validate.</typeparam>
/// <typeparam name="TResponse">The response type; must implement <see cref="IResult"/> so that validation errors can be returned.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class, IResult
{
    /// <summary>
    /// Runs all validators for the request; if valid, proceeds to the handler; otherwise returns an invalid result with errors.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!validators.Any())
            return await next(cancellationToken).ConfigureAwait(false);

        var context = new ValidationContext<TRequest>(request);
        var validationTasks = validators.Select(v => v.ValidateAsync(context, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks).ConfigureAwait(false);

        if (validationResults.All(x => x.IsValid))
            return await next(cancellationToken).ConfigureAwait(false);

        var errors = validationResults.SelectMany(x => x.AsErrors()).ToArray();

        return DynamicResults.Invalid<TResponse>(errors);
    }
}