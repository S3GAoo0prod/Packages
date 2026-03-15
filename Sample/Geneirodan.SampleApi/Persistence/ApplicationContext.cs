using Geneirodan.EntityFrameworkCore;
using Geneirodan.SampleApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Geneirodan.SampleApi.Persistence;

/// <summary>
/// Entity Framework Core DbContext for the sample API. Exposes <see cref="DomainEntity"/> as <see cref="DbSet"/>.
/// On construction, calls <see cref="DbContext.Database.EnsureCreated"/> so the database and schema are created if missing.
/// Used with <see cref="Repository{TEntity, TKey}"/> and <see cref="UnitOfWork"/> for the sample.
/// </summary>
public sealed class ApplicationContext : DbContext
{
    /// <summary>
    /// Initializes the context with the given options and ensures the database is created.
    /// </summary>
    /// <param name="options">The options (e.g. from dependency injection) specifying the database provider and connection.</param>
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) => Database.EnsureCreated();

    /// <summary>
    /// The set of <see cref="DomainEntity"/> entities. Used by the repository and for direct querying in tests or handlers.
    /// </summary>
    public DbSet<DomainEntity> DbSet { get; set; }

    /// <summary>
    /// Configures the <see cref="DomainEntity"/> model: primary key, seed data, and <see cref="DomainEntity.Name"/> max length.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DomainEntity>().HasData(
            new DomainEntity { Id = 1, Name = "Entity1" },
            new DomainEntity { Id = 2, Name = "Entity2" },
            new DomainEntity { Id = 3, Name = "Entity3" },
            new DomainEntity { Id = 4, Name = "Entity4" }
        );
        modelBuilder.Entity<DomainEntity>()
            .HasKey(x=>x.Id);

        modelBuilder.Entity<DomainEntity>()
            .Property(x => x.Name)
            .HasMaxLength(50);
        
        base.OnModelCreating(modelBuilder);
    }
}