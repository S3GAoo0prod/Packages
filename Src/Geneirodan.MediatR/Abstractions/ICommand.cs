using Ardalis.Result;
using JetBrains.Annotations;

namespace Geneirodan.MediatR.Abstractions;

/// <summary>
/// Marker and contract for a MediatR command that returns a non-generic <see cref="Result"/> (success or error, no value).
/// Implement as a sealed record with a nested <c>Handler</c> implementing <see cref="IRequestHandler{TRequest, TResponse}"/>.
/// Processed by the MediatR pipeline (logging, authorization, validation, exception handling) when sent via <c>ISender.Send</c>.
/// </summary>
/// <remarks>
/// <seealso cref="ICommand{T}"/> — command that returns a value.<br/>
/// <seealso cref="IQuery{T}"/> — query (read) contract with the same pipeline.
/// </remarks>
[PublicAPI]
public interface ICommand : IRequest<Result>;

/// <summary>
/// Marker and contract for a MediatR command that returns a <see cref="Result{T}"/> (success with value or error).
/// Use when the command produces a value (e.g. created entity ID or updated resource). Same pipeline and handler pattern as <see cref="ICommand"/>.
/// </summary>
/// <typeparam name="T">The type of the value returned on success (e.g. entity ID or DTO).</typeparam>
[PublicAPI]
public interface ICommand<T> : IRequest<Result<T>>;
