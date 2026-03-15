using Geneirodan.Abstractions.Domain;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Extension methods to register ASP.NET Core and Geneirodan.AspNetCore services: JWT authentication,
/// current user (<see cref="IUser"/>), localization, and central exception handling with problem details.
/// </summary>
[PublicAPI]
public static class DependencyInjection
{
    /// <summary>
    /// Registers JWT Bearer authentication as the default scheme. Configuration is bound from <paramref name="sectionName"/>.
    /// Use <paramref name="configureOptions"/> to override or add options (e.g. token validation). After this call, the HTTP context user
    /// is populated from the Bearer token.
    /// </summary>
    /// <param name="services">The service collection to add the authentication services to.</param>
    /// <param name="configureOptions">Optional delegate to configure <see cref="JwtBearerOptions"/>.</param>
    /// <param name="sectionName">The configuration section name for JWT options. Defaults to <c>JwtAuth</c>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with JWT authentication configured.</returns>
    /// <remarks>When <paramref name="sectionName"/> is used, the corresponding config section must contain valid JWT options (e.g. Authority, Audience).</remarks>
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
    /// Registers <see cref="IUser"/> as a scoped service implemented by <see cref="HttpUser"/>, which reads the current
    /// HTTP context claims. Requires HTTP context accessor and an authentication scheme (e.g. JWT or cookies) to populate the user.
    /// </summary>
    /// <param name="services">The service collection to add the user services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with <see cref="IUser"/> and <see cref="HttpUser"/> registered.</returns>
    public static IServiceCollection AddHttpUser(this IServiceCollection services) =>
        services
            .AddHttpContextAccessor()
            .AddScoped<IUser, HttpUser>();

    /// <summary>
    /// Registers request localization with the given supported cultures. The first culture in <paramref name="supportedCultures"/>
    /// is used as the default. Use this when the API or views need to vary by <see cref="System.Globalization.CultureInfo.CurrentCulture"/>.
    /// </summary>
    /// <param name="services">The service collection to add the localization services to.</param>
    /// <param name="supportedCultures">The list of supported culture names (e.g. "en", "ru"). The first is the default.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with localization and request localization configured.</returns>
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
    /// Registers <see cref="ExceptionHandler"/> as the application's exception handler and configures problem details
    /// so that Instance and requestId are set per request. When an unhandled exception occurs, the pipeline writes
    /// RFC 7807 problem details and an appropriate status code (see <see cref="ExceptionHandler"/>).
    /// </summary>
    /// <param name="services">The service collection to add the error handling services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with exception handler and problem details configured.</returns>
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
