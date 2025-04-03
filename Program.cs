using ASP.Net.UnitsNetSerializationExamples.Extensions;
using UnitsNet.Serialization.SystemTextJson.Value;

var builder = WebApplication.CreateBuilder(args);

var serializationOptions = builder.Configuration.GetSection("JsonConverter").Get<SerializationOptions>();

switch (serializationOptions.Serializer)
{
    case SerializerType.NewtonsoftJson:
    {
        builder.Services.AddControllersWithNewtonsoftConverter(serializationOptions.Schema);
        break;
    }
    case SerializerType.SystemTextJson:
    {
        var valueConverter = new QuantityValueDoubleConverter(); // TODO needs a switch in the serializerOptions
        builder.Services.AddControllersWithSystemTextJsonConverter(serializationOptions.Schema, valueConverter);
        break;
    }
    default:
        throw new ArgumentOutOfRangeException();
}

builder.Services.AddCustomSwagger(serializationOptions.Schema);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();