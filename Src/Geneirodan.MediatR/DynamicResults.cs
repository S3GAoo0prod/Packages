using Ardalis.Result;

namespace Geneirodan.MediatR;

/// <summary>
/// Provides utility methods to create dynamic results of type <see cref="Result"/> or <see cref="Result{T}"/>.
/// </summary>
public static class DynamicResults
{
    /// <summary>
    /// Creates an error result with the specified error list.
    /// </summary>
    /// <typeparam name="T">The result type, which must implement <see cref="IResult"/>.</typeparam>
    /// <param name="errors">An error list to include in the result.</param>
    /// <returns>An invalid result of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MissingMethodException">Thrown if the required method is not found on the result type.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result type is not
    /// <see cref="Result"/> or <see cref="Result{T}"/>.</exception>
    public static T Error<T>(ErrorList? errors = null) where T : class, IResult
    {
        var type = typeof(T);
        const string methodName = nameof(Result.Error);

        if (type.IsGenericResult())
            return type.CreateGenericResult<T>(methodName, [typeof(ErrorList)], [errors]);

        if (type.IsResult())
            return Result.Error(errors) as T ?? throw new MissingMethodException(type.Name, methodName);

        throw new InvalidOperationException("Validatable requests should return 'Result' or 'Result<T>'.");
    }

    /// <summary>
    /// Creates an invalid result with the specified validation errors.
    /// </summary>
    /// <typeparam name="T">The result type, which must implement <see cref="IResult"/>.</typeparam>
    /// <param name="errors">An array of validation errors to include in the result.</param>
    /// <returns>An invalid result of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MissingMethodException">Thrown if the required method is not found on the result type.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result type is not
    /// <see cref="Result"/> or <see cref="Result{T}"/>.</exception>
    public static T Invalid<T>(ValidationError[] errors) where T : class, IResult
    {
        var type = typeof(T);
        const string methodName = nameof(Result.Invalid);

        if (type.IsGenericResult())
            return type.CreateGenericResult<T>(methodName, [typeof(IEnumerable<ValidationError>)], [errors]);

        if (type.IsResult())
            return Result.Invalid(errors) as T ?? throw new MissingMethodException(type.Name, methodName);

        throw new InvalidOperationException("Validatable requests should return 'Result' or 'Result<T>'.");
    }

    /// <summary>
    /// Creates a forbidden result.
    /// </summary>
    /// <typeparam name="T">The result type, which must implement <see cref="IResult"/>.</typeparam>
    /// <returns>A forbidden result of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MissingMethodException">Thrown if the required method is not found on the result type.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result type is not
    /// <see cref="Result"/> or <see cref="Result{T}"/>.</exception>
    public static T Forbidden<T>() where T : class, IResult
    {
        var type = typeof(T);
        const string methodName = nameof(Result.Forbidden);

        if (type.IsGenericResult())
            return type.CreateGenericResult<T>(methodName, Type.EmptyTypes, []);

        if (type.IsResult())
            return Result.Forbidden() as T ?? throw new MissingMethodException(type.Name, methodName);

        throw new InvalidOperationException("Authorized requests should return 'Result' or 'Result<T>'.");
    }

    /// <summary>
    /// Creates an unauthorized result.
    /// </summary>
    /// <typeparam name="T">The result type, which must implement <see cref="IResult"/>.</typeparam>
    /// <returns>An unauthorized result of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MissingMethodException">Thrown if the required method is not found on the result type.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the result type is not
    /// <see cref="Result"/> or <see cref="Result{T}"/>.</exception>
    public static T Unauthorized<T>() where T : class, IResult
    {
        var type = typeof(T);
        const string methodName = nameof(Result.Unauthorized);

        if (type.IsGenericResult())
            return type.CreateGenericResult<T>(methodName, Type.EmptyTypes, []);

        if (type.IsResult())
            return Result.Unauthorized() as T ?? throw new MissingMethodException(type.Name, methodName);

        throw new InvalidOperationException("Authorized requests should return 'Result' or 'Result<T>'.");
    }

    /// <summary>
    /// Invokes a generic method on the result type to create a result.
    /// </summary>
    /// <typeparam name="T">The result type, which must implement <see cref="IResult"/>.</typeparam>
    /// <param name="type">The type of the result.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="types">The parameter types of the method.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>A result of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MissingMethodException">Thrown if the required method is not found on the result type.</exception>
    private static T CreateGenericResult<T>(this Type type, string methodName, Type[] types, object?[] parameters)
        where T : class, IResult =>
        type.GetGenericTypeDefinition()
            .MakeGenericType(type.GetGenericArguments())
            .GetMethod(methodName, types)?
            .Invoke(null, parameters) as T
        ?? throw new MissingMethodException(type.Name, methodName);

    /// <summary>
    /// Determines if the type is a non-generic <see cref="Result"/>.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is <see cref="Result"/>; otherwise, <c>false</c>.</returns>
    private static bool IsResult(this Type type) => type == typeof(Result);

    /// <summary>
    /// Determines if the type is a generic <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is <see cref="Result{T}"/>; otherwise, <c>false</c>.</returns>
    private static bool IsGenericResult(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>);
}