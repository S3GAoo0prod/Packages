using System.Reflection;
using Geneirodan.MediatR.Abstractions;
using Geneirodan.MediatR.Behaviors;
using Geneirodan.MediatR.Options;
using JetBrains.Annotations;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Geneirodan.MediatR;

/// <summary>
/// Extension methods to register MediatR and the Geneirodan.MediatR pipeline (logging, authorization, validation, exception handling).
/// Call <see cref="AddMediatRPipeline(IServiceCollection, Assembly[])"/> or the overload with <see cref="MediatRPipelineOptions"/> from the application startup.
/// Pass the assemblies that contain <see cref="ICommand"/> / <see cref="IQuery{T}"/> and their handlers.
/// </summary>
[PublicAPI]
public static class DependencyInjection
{
    /// <summary>
    /// Registers MediatR and the pipeline behaviors with default options (all behaviors enabled). Request and handler types are discovered from the given assemblies.
    /// </summary>
    /// <param name="services">The service collection to add MediatR and pipeline behaviors to.</param>
    /// <param name="assemblies">The assemblies to scan for request and handler types (e.g. <c>Assembly.GetExecutingAssembly()</c>).</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMediatRPipeline(this IServiceCollection services, params Assembly[] assemblies)
        => services.AddMediatRPipeline(new MediatRPipelineOptions(), assemblies);

    /// <summary>
    /// Registers MediatR and the pipeline behaviors with the given options. Use <paramref name="options"/> to disable logging, authorization, validation, or exception handling.
    /// </summary>
    /// <param name="services">The service collection to add MediatR and pipeline behaviors to.</param>
    /// <param name="options">Options that control which pipeline behaviors are registered.</param>
    /// <param name="assemblies">The assemblies to scan for request and handler types.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMediatRPipeline(
        this IServiceCollection services,
        MediatRPipelineOptions options,
        params Assembly[] assemblies
    ) => services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblies(assemblies);

        if (options.UseLogging)
            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingPreProcessor<>));

        if (options.UseAuthorization)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

        if (options.UseValidation)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        if (options.UseExceptions)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
    });
}