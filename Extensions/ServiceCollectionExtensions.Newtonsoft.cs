using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnitsNet.Serialization.JsonNet;

namespace ASP.Net.UnitsNetSerializationExamples.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    ///     Configures the application to use custom JSON serialization settings with Newtonsoft.Json.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the custom JSON settings will be added.</param>
    /// <param name="schema">
    ///     The <see cref="SerializationSchema" /> that determines the JSON serialization strategy to be used:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see cref="SerializationSchema.Abbreviated" />: Uses the
    ///                 <see cref="UnitsNet.Serialization.JsonNet.AbbreviatedUnitsConverter" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="SerializationSchema.UnitTypeAndName" />: Uses the
    ///                 <see cref="UnitsNet.Serialization.JsonNet.UnitsNetIQuantityJsonConverter" />.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="SerializationSchema.Default" />: Throws a <see cref="NotSupportedException" /> as
    ///                 no default implementation is available.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection" /> with the custom JSON settings applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided <paramref name="schema" /> is not recognized.</exception>
    /// <exception cref="NotSupportedException">
    ///     Thrown if <paramref name="schema" /> is set to
    ///     <see cref="SerializationSchema.Default" />.
    /// </exception>
    public static IServiceCollection AddControllersWithNewtonsoftConverter(this IServiceCollection services, SerializationSchema schema)
    {
        JsonConverter jsonConverter = schema switch
        {
            SerializationSchema.Abbreviated => new AbbreviatedUnitsConverter(),
            SerializationSchema.UnitTypeAndName => new UnitsNetIQuantityJsonConverter(),
            SerializationSchema.Default => throw new NotSupportedException("No default implementation available with Newtonsoft"),
            _ => throw new ArgumentOutOfRangeException()
        };
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Formatting = Formatting.Indented;
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.Converters.Add(jsonConverter);
        });
        return services;
    }
}