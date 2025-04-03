using ASP.Net.UnitsNetSerializationExamples.Filters;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Reflection;
using Newtonsoft.Json;
using UnitsNet.Serialization.SystemTextJson;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using System.Text.Json.Serialization;
using System.Text.Json;
using UnitsNet;

namespace ASP.Net.UnitsNetSerializationExamples.Extensions;

/// <summary>
/// Provides extension methods for configuring services in an ASP.NET Core application.
/// </summary>
/// <remarks>
/// This class includes methods for adding custom JSON serialization settings and configuring Swagger
/// with custom schema mappings for UnitsNet serialization.
/// </remarks>
public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the application to use Swagger with custom schema mappings and additional settings
    /// for UnitsNet serialization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which Swagger services will be added.</param>
    /// <param name="schema">
    /// The <see cref="SerializationSchema"/> that determines the schema mappings for UnitsNet types.
    /// Supported schemas include <see cref="SerializationSchema.Abbreviated"/> and <see cref="SerializationSchema.UnitTypeAndName"/>.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection"/> with Swagger services configured.</returns>
    /// <remarks>
    /// This method configures Swagger to include XML comments, enable annotations, and apply custom schema filters.
    /// Depending on the provided <paramref name="schema"/>, it applies specific schema mappings for UnitsNet types:
    /// - <see cref="SerializationSchema.Abbreviated"/>: Maps UnitsNet types with abbreviations.
    /// - <see cref="SerializationSchema.UnitTypeAndName"/>: Maps UnitsNet types with unit information.
    /// </remarks>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, SerializationSchema schema)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
            options.EnableAnnotations();
            
            options.SchemaFilter<EnumStringSchemaFilter>();

            options.MapType<QuantityInfo>(() => new OpenApiSchema(){Description = "Should not be mapped"});
            options.MapType<UnitInfo>(() => new OpenApiSchema(){Description = "Should not be mapped"});
            options.MapType<UnitKey>(() => new OpenApiSchema(){Description = "Should not be mapped"});
            options.MapType<BaseDimensions>(() => new OpenApiSchema(){Description = "Should not be mapped"});
            if (schema is SerializationSchema.Abbreviated)
            {
                // options.SchemaFilter<AbbreviatedQuantitySchemaFilter>();
                // options.SchemaFilter<AbbreviatedInterfaceQuantitySchemaFilter>();
                // options.MapType<Density>(() => new OpenApiSchema(){Description = "Test", });
                // options.MapType<IQuantity>(() => new OpenApiSchema(){Description = "Test IQuantity", });
                options.SchemaGeneratorOptions.CustomTypeMappings =
                    new Dictionary<Type, Func<OpenApiSchema>>()
                        .AddIQuantityWithAbbreviationMapping(CustomTypeMappingSwaggerExtension.ToOpenApiSchemaWithAbbreviations)
                        .WithAdditionalResourcesInfo();
            }
            else if (schema is SerializationSchema.UnitTypeAndName)
            {
                options.SchemaGeneratorOptions.CustomTypeMappings =
                    new Dictionary<Type, Func<OpenApiSchema>>()
                        .AddIQuantityMapping(CustomTypeMappingSwaggerExtension.ToOpenApiSchemaWithUnits)
                        .WithAdditionalResourcesInfo();
            }

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "UnitsNet serialization examples, with additional custom mapping",
                Description = $"Using converter schema: {schema}"
            });
        });
        return services;
    }
}