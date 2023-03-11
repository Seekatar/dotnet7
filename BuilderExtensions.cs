using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace dotnet7;

internal static class BuilderExtensions
{
    // from .NET Conf 2022 sample https://github.com/captainsafia/TrainingApi/blob/42a26a8164175fc1570b2caed4f69c77fb6c14a0/TrainingApi/Program.cs
    public static TBuilder EnableOpenApiWithAuthentication<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        };
        builder.WithOpenApi(operation => new(operation)
        {
            Security =
            {
                new()
                {
                    [scheme] = new List<string>()
                }
            }
        });
        return builder;
    }
}
