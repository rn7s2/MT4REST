using TradingAPI.MT4Server;

namespace MT4REST.Models;

public class Account
{
    public ulong id { get; set; }
    public QuoteClient qc { get; set; }
    public OrderClient oc { get; set; }

    public Account(ulong id, QuoteClient qc)
    {
        this.id = id;
        this.qc = qc;
        this.oc = new OrderClient(qc);
    }
}
