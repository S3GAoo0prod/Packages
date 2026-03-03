using Ardalis.Result;
using Geneirodan.Abstractions.Repositories;
using JetBrains.Annotations;

namespace Geneirodan.MediatR.Abstractions;

/// <summary>
/// Marker and contract for a MediatR query that returns a <see cref="Result{T}"/> (success with data or error).
/// Implement as a sealed record with a nested <c>Handler</c>. Use for read operations; processed by the same pipeline as commands
/// (logging, authorization, validation, exception handling) when sent via <c>ISender.Send</c>.
/// </summary>
/// <typeparam name="T">The type of the data returned on success (e.g. entity, DTO, or <see cref="PageModel{T}"/>).</typeparam>
[PublicAPI]
public interface IQuery<T> : IRequest<Result<T>>;