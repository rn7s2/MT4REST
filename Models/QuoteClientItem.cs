using TradingAPI.MT4Server;

namespace MT4REST.Models;

public class QuoteClientItem
{
    public ulong id { get; set; }
    public QuoteClient qc { get; set; }

    public QuoteClientItem(ulong id, QuoteClient qc)
    {
        this.id = id;
        this.qc = qc;
    }
}
