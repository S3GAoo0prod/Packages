using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Geneirodan.AspNetCore;

/// <summary>
/// OpenAPI document transformer that adds the JWT Bearer security scheme and applies it to all operations when JWT authentication is configured.
/// Used by Swagger/OpenAPI UI so that "Authorize" uses the Bearer token. Registers the scheme in <c>document.Components.SecuritySchemes</c>
/// and adds the security requirement to each operation so that generated clients send the Authorization header.
/// </summary>
[PublicAPI]
public sealed class JwtBearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    private static readonly OpenApiSecurityScheme OpenApiSecurityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        BearerFormat = "Json Web Token"
    };

    private static readonly OpenApiSecurityRequirement Requirement = new()
    {
        [
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            }
        ] = []
    };

    /// <summary>
    /// Adds JWT Bearer security scheme and requirement to the OpenAPI document if the JWT Bearer authentication scheme is registered.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The transformer context.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync().ConfigureAwait(false);
        if (authenticationSchemes.Any(authScheme => string.Equals(
                a: authScheme.Name,
                b: JwtBearerDefaults.AuthenticationScheme,
                comparisonType: StringComparison.Ordinal
            )))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>(StringComparer.Ordinal)
            {
                [JwtBearerDefaults.AuthenticationScheme] = OpenApiSecurityScheme
            };

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
                operation.Value.Security.Add(Requirement);
        }
    }
}