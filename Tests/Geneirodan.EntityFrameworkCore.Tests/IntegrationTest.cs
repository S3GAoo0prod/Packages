using Geneirodan.SampleApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geneirodan.EntityFrameworkCore.Tests;

/// <summary>
/// Base class for integration tests that use the sample API host (<see cref="ApiFactory"/>) and a scoped DbContext.
/// Provides <see cref="Context"/> for direct DB access and <see cref="Scope"/> for resolving services (e.g. repository, unit of work).
/// </summary>
public abstract class IntegrationTest : IClassFixture<ApiFactory>, IDisposable
{
    protected IntegrationTest(ApiFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        Context = Scope.ServiceProvider.GetRequiredService<DbContext>();
    }

    protected readonly WebApplicationFactory<IApiMarker> Factory;
    protected IServiceScope Scope;
    protected DbContext Context;

    public void Dispose()
    {
        Scope.Dispose();
        GC.SuppressFinalize(this);
    }
}