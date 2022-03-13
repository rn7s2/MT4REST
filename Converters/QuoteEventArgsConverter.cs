using TradingAPI.MT4Server;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MT4REST.Converters;

public class QuoteEventArgsConverter : JsonConverter<QuoteEventArgs>
{
    public override QuoteEventArgs Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, QuoteEventArgs value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("ask", value.Ask);
        writer.WriteNumber("bid", value.Bid);
        writer.WriteString("symbol", value.Symbol);
        writer.WriteString("time", value.Time);
        writer.WriteEndObject();
    }
}