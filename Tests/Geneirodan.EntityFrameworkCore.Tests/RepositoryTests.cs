using Geneirodan.Abstractions.Repositories;
using Geneirodan.SampleApi.Domain;
using Geneirodan.SampleApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Geneirodan.EntityFrameworkCore.Tests;

public sealed class RepositoryTests : IntegrationTest, IAsyncDisposable
{
    private IRepository<DomainEntity, int> _repository;
    private IUnitOfWork _unitOfWork;

    public RepositoryTests(ApiFactory factory) : base(factory)
    {
        _repository = Scope.ServiceProvider.GetRequiredService<IRepository<DomainEntity, int>>();
        _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    [Fact]
    public async Task FindAsync_ReturnsMappedEntity_WhenEntityExists()
    {
        var entity = await CreateEntity();

        var result = await _repository.FindAsync(entity.Id, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(entity);
    }

    private async Task<DomainEntity> CreateEntity(int id = 0, string name = "Entity1")
    {
        var entity = new DomainEntity { Id = id, Name = name };
        Context.Set<DomainEntity>().Add(entity);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        return entity;
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
    {
        var entity = await CreateEntity();
        var result = await _repository.ExistsAsync(entity.Id, TestContext.Current.CancellationToken);
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenEntityDoesNotExists()
    {
        var result = await _repository.ExistsAsync(128, TestContext.Current.CancellationToken);
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity_WhenSaveChangesCalled()
    {
        var entity = new DomainEntity { Id = 1, Name = "Entity1" };

        var result = await _repository.AddAsync(entity, TestContext.Current.CancellationToken);
        await _unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(entity);

        RefreshScope();

        entity = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        entity.ShouldNotBeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldNotAddEntity_WhenSaveChangesNotCalled()
    {
        var entity = new DomainEntity { Id = 1, Name = "Entity1" };

        var result = await _repository.AddAsync(entity, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();

        RefreshScope();

        entity = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        entity.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity_WhenSaveChangesCalled()
    {
        var entity = await CreateEntity();

        entity.Name = "UpdateEntity";

        var result = await _repository.UpdateAsync(entity, TestContext.Current.CancellationToken);
        await _unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(entity);

        RefreshScope();

        entity = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        entity.ShouldNotBeNull();
        entity.Name.ShouldBe("UpdateEntity");
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotUpdateEntity_WhenSaveChangesNotCalled()
    {
        var entity = await CreateEntity(name: "NotUpdateEntity");

        entity.Name = "UpdateEntity";

        var result = await _repository.UpdateAsync(entity, TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(entity);

        RefreshScope();

        entity = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        entity.ShouldNotBeNull();
        entity.Name.ShouldBe("NotUpdateEntity");
    }

    private void RefreshScope()
    {
        Scope.Dispose();
        Scope = Factory.Services.CreateScope();
        Context = Scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        _repository = Scope.ServiceProvider.GetRequiredService<IRepository<DomainEntity, int>>();
        _unitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteEntity_WhenSaveChangesCalled()
    {
        var entity = await CreateEntity();

        await _repository.DeleteAsync(entity, TestContext.Current.CancellationToken);
        await _unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

        RefreshScope();

        var result = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotDeleteEntity_WhenSaveChangesNotCalled()
    {
        var entity = await CreateEntity();

        await _repository.DeleteAsync(entity, TestContext.Current.CancellationToken);

        RefreshScope();

        var result = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Transaction_ShouldSaveChanges_WhenCommited()
    {
        var entity = await CreateEntity();
        
        await using (var transaction = await _unitOfWork.BeginTransactionAsync(TestContext.Current.CancellationToken))
        {
            await _repository.DeleteAsync(entity, TestContext.Current.CancellationToken);
            await _unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);
            await transaction.CommitAsync(TestContext.Current.CancellationToken);
        }

        RefreshScope();

        var result = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Transaction_ShouldNotSaveChanges_WhenNotCommited()
    {
        var entity = await CreateEntity();

        await using (await _unitOfWork.BeginTransactionAsync(TestContext.Current.CancellationToken))
        {
            await _repository.DeleteAsync(entity, TestContext.Current.CancellationToken);
            await _unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
        
        RefreshScope();

        var result = await Context.Set<DomainEntity>()
            .FirstOrDefaultAsync(x => x.Id.Equals(entity.Id), TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
    }

    public async ValueTask DisposeAsync()
    {
        Context.Set<DomainEntity>().RemoveRange(Context.Set<DomainEntity>());
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}