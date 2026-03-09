using Geneirodan.Abstractions.Domain;

namespace Geneirodan.SampleApi.Domain;

public class DomainEntity : IEntity<int>
{
    public int Id { get; init; }
    public required string Name { get; set; }
}