using MT4REST.Converters;
using System.Text.Json;

namespace MT4REST.Services;
public static class JsonSerializerOptionsService
{
    public static JsonSerializerOptions order = new JsonSerializerOptions();

    static JsonSerializerOptionsService()
    {
        order.Converters.Add(new OrderJsonConverter());
    }
}
