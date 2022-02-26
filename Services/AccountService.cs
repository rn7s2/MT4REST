using MT4REST.Models;
using TradingAPI.MT4Server;

namespace MT4REST.Services;

public static class AccountService
{
    private static List<Account> accounts;
    public static ulong nextId = 1;

    static AccountService()
    {
        accounts = new List<Account>();
    }

    public static ulong Add(QuoteClient qc)
    {
        accounts.Add(new Account(nextId, qc));
        return nextId++;
    }

    public static Account? Get(ulong id)
    {
        foreach (var account in accounts)
        {
            if (account.id == id)
                return account;
        }
        return null;
    }

    public static void Delete(ulong id)
    {
        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].id != id)
                continue;
            accounts.RemoveAt(i);
            break;
        }
    }
}
