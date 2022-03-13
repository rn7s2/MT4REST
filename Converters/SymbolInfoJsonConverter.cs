using TradingAPI.MT4Server;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MT4REST.Converters;

public class SymbolInfoJsonConverter : JsonConverter<SymbolInfo>
{
    public override SymbolInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, SymbolInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("code", value.Code);
        writer.WriteNumber("contractSize", value.ContractSize);
        writer.WriteString("currency", value.Currency);
        writer.WriteNumber("digits", value.Digits);
        writer.WriteString("execution", "" + value.Execution);
        writer.WriteNumber("freezeLevel", value.FreezeLevel);
        writer.WriteString("marginCurrency", value.MarginCurrency);
        writer.WriteNumber("marginDivider", value.MarginDivider);
        writer.WriteString("marginMode", "" + value.MarginMode);
        writer.WriteNumber("point", value.Point);
        writer.WriteString("profitMode", "" + value.ProfitMode);
        writer.WriteNumber("spread", value.Spread);
        writer.WriteNumber("stopsLevel", value.StopsLevel);
        writer.WriteNumber("swapLong", value.SwapLong);
        writer.WriteNumber("swapShort", value.SwapShort);
        writer.WriteEndObject();
    }
}
