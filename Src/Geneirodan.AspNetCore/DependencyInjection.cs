using Geneirodan.Abstractions.Domain;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Provides extension methods for configuring services in <see cref="IServiceCollection"/>.
/// </summary>
[PublicAPI]
public static class DependencyInjection
{
    /// <summary>
    /// Adds JWT authentication services to the <see cref="IServiceCollection"/> using a specified metadata address.
    /// </summary>
    /// <param name="services">The service collection to add the authentication services to.</param>
    /// <param name="configureOptions"></param>
    /// <param name="sectionName">The section name for the JWT bearer token configuration.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with JWT authentication configured.</returns>
    public static IServiceCollection AddJwtAuth(
        this IServiceCollection services, 
        Action<JwtBearerOptions>? configureOptions = null,
        string sectionName = "JwtAuth"
        )
    {
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        var authenticationBuilder = services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        
        if (configureOptions is null)
            authenticationBuilder.AddJwtBearer();
        else
            authenticationBuilder.AddJwtBearer(configureOptions);

        return services;
    }

    /// <summary>
    /// Adds HTTP implementation of <see cref="IUser"/> services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the user services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with HTTP user service added.</returns>
    public static IServiceCollection AddHttpUser(this IServiceCollection services) =>
        services
            .AddHttpContextAccessor()
            .AddScoped<IUser, HttpUser>();

    /// <summary>
    /// Adds web localization services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the localization services to.</param>
    /// <param name="supportedCultures">A list of supported cultures for localization.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with localization services configured.</returns>
    public static IServiceCollection AddWebLocalization(this IServiceCollection services,
        params string[] supportedCultures) =>
        services
            .AddLocalization()
            .AddRequestLocalization(options =>
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures)
            );

    /// <summary>
    /// Adds error handling services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the error handling services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with error handling services configured.</returns>
    public static IServiceCollection AddErrorHandling(this IServiceCollection services) =>
        services
            .AddExceptionHandler<ExceptionHandler>()
            .AddProblemDetails(options =>
                options.CustomizeProblemDetails = context =>
                {
                    var httpContext = context.HttpContext;
                    context.ProblemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);
                });
}
