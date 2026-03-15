using Ardalis.Result;
using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;

namespace Geneirodan.MediatR.Abstractions;

/// <summary>
/// Marker and contract for a MediatR query that returns a <see cref="Result{T}"/> (success with data or error).
/// Use for read operations.
/// </summary>
/// <typeparam name="T">The type of the data returned on success (e.g. entity, DTO, or <see cref="PageModel{T}"/>).</typeparam>
[PublicAPI]
public interface IQuery<T> : IRequest<Result<T>>;