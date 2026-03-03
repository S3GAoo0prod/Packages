namespace Geneirodan.Abstractions.Mapping;

/// <summary>
/// Defines a contract for mapping an object of type <typeparamref name="TSource"/> to <typeparamref name="TDestination"/>.
/// Used to separate domain entities from DTOs or API models; implementations can be hand-written or backed by a mapping library.
/// The EntityFrameworkCore package provides <c>ProjectTo</c> extension methods that use this interface for queryable-to-queryable mapping.
/// </summary>
/// <typeparam name="TSource">The type of the source object to be mapped (e.g. entity or domain model).</typeparam>
/// <typeparam name="TDestination">The type of the destination object after mapping (e.g. DTO or response model).</typeparam>
public interface IMapper<in TSource, out TDestination>
{
    /// <summary>
    /// Maps the source object to a destination object. Implementations must not return <see langword="null"/> unless <typeparamref name="TDestination"/> is nullable.
    /// </summary>
    /// <param name="source">The source object to be mapped.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map(TSource source);
}