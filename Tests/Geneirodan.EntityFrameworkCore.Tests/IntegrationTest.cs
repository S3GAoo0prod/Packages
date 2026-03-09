using Geneirodan.SampleApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geneirodan.EntityFrameworkCore.Tests;

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