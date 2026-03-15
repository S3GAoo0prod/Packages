using Ardalis.Result;
using JetBrains.Annotations;

namespace Geneirodan.MediatR.Abstractions;

/// <summary>
/// Marker and contract for a MediatR command that returns a non-generic <see cref="Result"/> (success or error, no value).
/// </summary>
/// <remarks>
/// <seealso cref="ICommand{T}"/> — command that returns a value.<br/>
/// <seealso cref="IQuery{T}"/> — query (read) contract.
/// </remarks>
[PublicAPI]
public interface ICommand : IRequest<Result>;

/// <summary>
/// Marker and contract for a MediatR command that returns a <see cref="Result{T}"/> (success with value or error).
/// Use when the command produces a value (e.g. created entity ID or updated resource).
/// </summary>
/// <typeparam name="T">The type of the value returned on success (e.g. entity ID or DTO).</typeparam>
[PublicAPI]
public interface ICommand<T> : IRequest<Result<T>>;
