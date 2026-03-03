using Geneirodan.Abstractions.Mapping;
using JetBrains.Annotations;

namespace Geneirodan.EntityFrameworkCore;

/// <summary>
/// Extension methods for mapping <see cref="IQueryable{T}"/> to another queryable type using <see cref="IMapper{TSource, TDestination}"/>.
/// Use when the mapper can translate the entire query (e.g. to a different entity or DTO) so that the database executes a single projected query.
/// </summary>
[PublicAPI]
public static class MappingExtensions
{
    /// <summary>
    /// Projects the queryable of <typeparamref name="TSource"/> to <see cref="IQueryable{TDestination}"/> using the provided mapper.
    /// The mapper must produce a queryable that can be executed by the provider (e.g. EF Core); execution is deferred until the query is enumerated.
    /// </summary>
    /// <typeparam name="TSource">The type of the source entities in the queryable.</typeparam>
    /// <typeparam name="TDestination">The type of the destination after mapping.</typeparam>
    /// <param name="queryable">The source queryable to map.</param>
    /// <param name="mapper">The mapper that converts the queryable to the destination queryable (e.g. entity to DTO projection).</param>
    /// <returns>An <see cref="IQueryable{TDestination}"/> that, when executed, returns the mapped results.</returns>
    public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        IMapper<IQueryable<TSource>, IQueryable<TDestination>> mapper
    ) => mapper.Map(queryable);
}