using TradingAPI.MT4Server;

namespace MT4REST.Models;

public class SymbolParams
{
    public SymbolInfo symbol { get; set; }
    public ConSymbolGroup group { get; set; }
    public ConGroupSec groupParams { get; set; }

    public SymbolParams(SymbolInfo symbolInfo, ConSymbolGroup conSymbolGroup, ConGroupSec conGroupSec)
    {
        this.symbol = symbolInfo;
        this.group = conSymbolGroup;
        this.groupParams = conGroupSec;
    }
}
