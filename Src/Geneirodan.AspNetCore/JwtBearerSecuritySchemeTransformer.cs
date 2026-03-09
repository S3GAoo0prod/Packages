using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Geneirodan.AspNetCore;

/// <summary>
/// Represents a transformer that can be used to modify an OpenAPI document to add JWT bearer security schema.
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

    /// <inheritdoc />
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