using MT4REST.Models;
using TradingAPI.MT4Server;

namespace MT4REST.Services;

public static class QuoteClientService
{
    private static List<QuoteClientItem> qcs;
    public static ulong nextId = 1;

    static QuoteClientService()
    {
        qcs = new List<QuoteClientItem>();
    }

    public static ulong Add(QuoteClient qc)
    {
        qcs.Add(new QuoteClientItem(nextId, qc));
        return nextId++;
    }

    public static QuoteClient? Get(ulong id)
    {
        foreach (var qc in qcs)
        {
            if (qc.id == id)
                return qc.qc;
        }
        return null;
    }

    public static void Delete(ulong id)
    {
        for (int i = 0; i < qcs.Count; i++)
        {
            if (qcs[i].id != id)
                continue;
            qcs.RemoveAt(i);
            break;
        }
    }
}
