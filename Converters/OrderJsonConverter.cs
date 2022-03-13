using TradingAPI.MT4Server;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MT4REST.Converters;

public class OrderJsonConverter : JsonConverter<Order>
{
    public override Order Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Order value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("ticket", value.Ticket);
        writer.WriteNumber("rateMargin", value.RateMargin);
        writer.WriteNumber("rateClose", value.RateClose);
        writer.WriteNumber("rateOpen", value.RateOpen);
        writer.WriteNumber("profit", value.Profit);
        writer.WriteString("comment", value.Comment);
        writer.WriteNumber("commission", value.Commission);
        writer.WriteNumber("swap", value.Swap);
        writer.WriteNumber("closePrice", value.ClosePrice);
        writer.WriteNumber("magicNumber", value.MagicNumber);
        writer.WriteNumber("stopLoss", value.StopLoss);
        writer.WriteNumber("openPrice", value.OpenPrice);
        writer.WriteString("symbol", value.Symbol);
        writer.WriteNumber("lots", value.Lots);
        writer.WriteString("type", "" + value.Type);
        writer.WriteString("expiration", value.Expiration.ToString("yyyy-MM-dd HH:mm:ss"));
        writer.WriteString("closeTime", value.CloseTime.ToString("yyyy-MM-dd HH:mm:ss"));
        writer.WriteString("openTime", value.OpenTime.ToString("yyyy-MM-dd HH:mm:ss"));
        writer.WriteNumber("takeProfit", value.TakeProfit);
        writer.WriteEndObject();
    }
}
