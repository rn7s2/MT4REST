using TradingAPI.MT4Server;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MT4REST.Converters;

public class BarJsonConverter : JsonConverter<Bar>
{
    public override Bar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Bar value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("close", value.Close);
        writer.WriteNumber("high", value.High);
        writer.WriteNumber("low", value.Low);
        writer.WriteNumber("open", value.Open);
        writer.WriteString("time", value.Time.ToString("yyyy-MM-dd HH:mm:ss"));
        writer.WriteNumber("volume", value.Volume);
        writer.WriteEndObject();
    }
}