using MT4REST.Converters;
using System.Text.Json;

namespace MT4REST.Services;
public static class JsonSerializerOptionsService
{
    public static JsonSerializerOptions converter = new JsonSerializerOptions();

    static JsonSerializerOptionsService()
    {
        converter.Converters.Add(new OrderJsonConverter());
        converter.Converters.Add(new SymbolInfoJsonConverter());
        converter.Converters.Add(new BarJsonConverter());
    }
}
