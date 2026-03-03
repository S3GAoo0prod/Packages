using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Geneirodan.MediatR.Tests;

/// <summary>
/// Base class for tests that exercise the MediatR pipeline (commands/queries and behaviors) using the sample API host.
/// Provides <see cref="Sender"/> to send requests and <see cref="Scope"/> for resolving services.
/// </summary>
public abstract class PipelineTest : IClassFixture<ApiFactory>, IDisposable
{
    protected readonly ISender Sender;
    protected readonly IServiceScope Scope;

    protected PipelineTest(ApiFactory factory)
    {
        Scope = factory.Services.CreateScope();
        Sender = Scope.ServiceProvider.GetRequiredService<ISender>();
    }

    public void Dispose()
    {
        Scope.Dispose();
        GC.SuppressFinalize(this);
    }
}