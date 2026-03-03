using Geneirodan.Abstractions.Domain;

namespace Geneirodan.SampleApi.Domain;

/// <summary>
/// Sample domain entity used by the example API and persistence layer.
/// Implements <see cref="IEntity{TKey}"/> with integer key so it can be stored via the repository abstraction
/// and mapped in <see cref="Persistence.ApplicationContext"/>.
/// </summary>
public class DomainEntity : IEntity<int>
{
    /// <summary>
    /// Unique identifier of the entity. Set by the database or seed data.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Display name of the entity. Required and limited in length by the persistence configuration.
    /// </summary>
    public required string Name { get; set; }
}