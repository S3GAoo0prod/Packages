using Ardalis.Result;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using IResult = Ardalis.Result.IResult;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Provides extension methods to convert an <see cref="Ardalis.Result.IResult"/>
/// into an ASP.NET Core <see cref="Microsoft.AspNetCore.Http.IResult"/>.
/// </summary>
[PublicAPI]
public static class ResultConverter 
{
    /// <summary>
    /// Maps the given <see cref="Ardalis.Result.IResult"/>
    /// to an appropriate ASP.NET Core <see cref="Microsoft.AspNetCore.Http.IResult"/>.
    /// </summary>
    /// <param name="result">The <see cref="Ardalis.Result.IResult"/> to map.</param>
    /// <returns>An <see cref="Microsoft.AspNetCore.Http.IResult"/> representing the HTTP response.</returns>
    /// <exception cref="NotSupportedException">Thrown when the result status is not supported.</exception>
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
