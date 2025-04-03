using System.Text.Json.Serialization;
using UnitsNet;
using UnitsNet.Serialization.SystemTextJson;
using UnitsNet.Serialization.SystemTextJson.Unit;

namespace ASP.Net.UnitsNetSerializationExamples.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds controllers to the application with custom JSON serialization settings using System.Text.Json.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the controllers and custom JSON settings will be
    ///     added.
    /// </param>
    /// <param name="schema">
    ///     Specifies the <see cref="SerializationSchema" /> to determine the JSON serialization strategy for UnitsNet types.
    /// </param>
    /// <param name="valueConverter">
    ///     A custom <see cref="System.Text.Json.Serialization.JsonConverter{T}" /> for serializing and deserializing
    ///     <see cref="UnitsNet.QuantityValue" />.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection" /> with controllers and custom JSON serialization settings applied.</returns>
    /// <remarks>
    ///     This method configures the application to use custom JSON converters for UnitsNet types based on the specified
    ///     <paramref name="schema" />. It modifies the <see cref="System.Text.Json.JsonSerializerOptions.Converters" />
    ///     collection
    ///     to include the appropriate converters for handling UnitsNet quantities.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     Thrown when an unsupported <paramref name="schema" /> value is provided.
    /// </exception>
    public static IServiceCollection AddControllersWithSystemTextJsonConverter(this IServiceCollection services, SerializationSchema schema,
        JsonConverter<QuantityValue> valueConverter)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            var converters = options.JsonSerializerOptions.Converters;
            switch (schema)
            {
                case SerializationSchema.Default:
                    converters.Add(new JsonQuantityConverter());
                    converters.Add(new JsonStringEnumConverter());
                    converters.Add(valueConverter);
                    break;
                case SerializationSchema.Abbreviated:
                    converters.Add(new AbbreviatedQuantityConverter(valueConverter));
                    converters.Add(new AbbreviatedInterfaceQuantityConverter.WithValueConverter(valueConverter));
                    break;
                case SerializationSchema.UnitTypeAndName:
                    converters.Add(new JsonQuantityConverter());
                    converters.Add(new UnitTypeAndNameConverter());
                    converters.Add(new InterfaceQuantityWithUnitTypeConverter());
                    converters.Add(valueConverter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
        return services;
    }
}