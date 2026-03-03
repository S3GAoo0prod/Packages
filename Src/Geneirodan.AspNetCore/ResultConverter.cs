using Ardalis.Result;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using IResult = Ardalis.Result.IResult;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Converts <see cref="Ardalis.Result.IResult"/> (returned by MediatR handlers) into ASP.NET Core minimal API <see cref="Microsoft.AspNetCore.Http.IResult"/>.
/// Use <see cref="MapResult(IResult)"/> in endpoint lambdas after sending a command or query so that Ok/NotFound/ValidationProblem
/// and other results are written correctly. Maps <see cref="ResultStatus"/> to HTTP status codes and problem details.
/// </summary>
[PublicAPI]
public static class ResultConverter
{
    /// <summary>
    /// Maps the given <see cref="Ardalis.Result.IResult"/> to the corresponding ASP.NET Core <see cref="Microsoft.AspNetCore.Http.IResult"/>.
    /// Handles Ok, Created, NoContent, NotFound, Unauthorized, Forbidden, Invalid (validation errors), Error, Conflict, Unavailable, and CriticalError.
    /// </summary>
    /// <param name="result">The result returned from a MediatR handler (e.g. <see cref="Result"/> or <see cref="Result{T}"/>).</param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> that writes the appropriate HTTP response for <paramref name="result"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="result"/> has a <see cref="ResultStatus"/> that is not mapped to an HTTP result.</exception>
    public static Microsoft.AspNetCore.Http.IResult MapResult(this IResult result) => result.Status switch
    {
        ResultStatus.Ok => result is Result ? TypedResults.Ok() : TypedResults.Ok(result.GetValue()),
        ResultStatus.Created => TypedResults.Created(result.Location, result.GetValue()),
        ResultStatus.NoContent => TypedResults.NoContent(),
        ResultStatus.NotFound => Error(result, StatusCodes.Status404NotFound),
        ResultStatus.Unauthorized => Error(result, StatusCodes.Status401Unauthorized),
        ResultStatus.Forbidden => Error(result, StatusCodes.Status403Forbidden),
        ResultStatus.Invalid => ValidationError(result.ValidationErrors),
        ResultStatus.Error => Error(result, StatusCodes.Status422UnprocessableEntity),
        ResultStatus.Conflict => Error(result, StatusCodes.Status409Conflict),
        ResultStatus.Unavailable => Error(result, StatusCodes.Status503ServiceUnavailable),
        ResultStatus.CriticalError => Error(result, StatusCodes.Status500InternalServerError),
        _ => throw new NotSupportedException($"Result {result.Status} conversion is not supported.")
    };

    /// <summary>
    /// Converts a collection of validation errors into a <see cref="ValidationProblem"/> response.
    /// </summary>
    /// <param name="resultValidationErrors">The validation errors to convert.</param>
    /// <returns>A <see cref="ValidationProblem"/> containing the validation errors.</returns>
    private static ValidationProblem ValidationError(IEnumerable<ValidationError> resultValidationErrors)
    {
        var errors = resultValidationErrors
            .GroupBy(x => x.Identifier, x => x.ErrorMessage, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.ToArray(), StringComparer.Ordinal);
        return TypedResults.ValidationProblem(errors);
    }

    /// <summary>
    /// Creates a <see cref="ProblemHttpResult"/> for error responses.
    /// </summary>
    /// <param name="result">The <see cref="IResult"/> containing error details.</param>
    /// <param name="statusCode">The HTTP status code to use in the response.</param>
    /// <returns>A <see cref="ProblemHttpResult"/> representing the error response.</returns>
    private static ProblemHttpResult Error(IResult result, int? statusCode)
    {
        var extensions = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        if (result.Errors.Any())
            extensions.TryAdd("errors", result.Errors);
        return TypedResults.Problem(statusCode:statusCode, extensions: extensions);
    }
}
