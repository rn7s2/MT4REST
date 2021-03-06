using MT4REST.Services;
using MT4REST.Models;
using TradingAPI.MT4Server;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MT4REST.Controllers;

[ApiController]
[Route("[controller]")]
public class MT4Controller : ControllerBase
{
    private readonly ILogger<MT4Controller> _logger;
    private readonly string _srvFilePath;

    public MT4Controller(ILogger<MT4Controller> logger)
    {
        _logger = logger;
        _srvFilePath = Path.GetTempFileName();
    }

    [HttpGet]
    [Route("Ping")]
    public ActionResult<int> GetPing(string host)
    {
        return CreatedAtAction(nameof(GetPing), QuoteClient.PingHost(host));
    }

    [HttpPost]
    [Route("LoadSrv")]
    public ActionResult<Server[]> PostLoadSrv(IFormFile file)
    {
        FileStream stream = new FileStream(_srvFilePath, FileMode.Create);
        file.CopyTo(stream);
        stream.Close();

        Server[] SrvList;
        QuoteClient.LoadSrv(_srvFilePath, out SrvList);

        System.IO.File.Delete(_srvFilePath);

        return CreatedAtAction(nameof(PostLoadSrv), SrvList);
    }

    [HttpPost]
    [Route("Connect")]
    public ActionResult<ulong> PostConnect(int user, string password, IFormFile file)
    {
        FileStream stream = new FileStream(_srvFilePath, FileMode.Create);
        file.CopyTo(stream);
        stream.Close();

        Server[] SrvList;
        QuoteClient.LoadSrv(_srvFilePath, out SrvList);

        System.IO.File.Delete(_srvFilePath);

        QuoteClient qc = new QuoteClient();
        qc.PathForSavingSrv = ".";
        qc.CalculateTradeProps = true;
        qc.SetAutoReconnect(true);

        try
        {
            qc.Init(user, password, SrvList[0].Host, SrvList[0].Port);
            qc.Connect();

            Console.WriteLine("Connected to server.");
            Console.WriteLine("Server build: " + qc.ServerBuild);
            while (qc.ServerTime == DateTime.MinValue)
                Thread.Sleep(10);
            Console.WriteLine("Server time: " + qc.ServerTime);

            return CreatedAtAction(nameof(PostConnect), AccountService.Add(qc));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());

            for (int i = 0; i < SrvList.Length; i++)
            {
                qc = new QuoteClient(user, password, SrvList[i].Host, SrvList[i].Port);
                try
                {
                    Console.WriteLine("Trying slave server #" + i);
                    qc.Connect();

                    return CreatedAtAction(nameof(PostConnect), AccountService.Add(qc));
                }
                catch (Exception _e)
                {
                    Console.WriteLine(_e.ToString());
                }
            }

            Console.WriteLine("No servers to try now, connection failed.");
            return StatusCode(403);
        }
    }

    [HttpGet]
    [Route("Connect")]
    public ActionResult<ulong> GetConnect(int user, string password, string host, int port)
    {
        QuoteClient qc = new QuoteClient(user, password, host, port);
        qc.PathForSavingSrv = ".";
        qc.CalculateTradeProps = true;
        qc.SetAutoReconnect(true);

        try
        {
            qc.Connect();

            Console.WriteLine("Connected to server.");
            Console.WriteLine("Server build: " + qc.ServerBuild);
            while (qc.ServerTime == DateTime.MinValue)
                Thread.Sleep(10);
            Console.WriteLine("Server time: " + qc.ServerTime);

            return CreatedAtAction(nameof(GetConnect), AccountService.Add(qc));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());

            return StatusCode(403);
        }
    }

    [HttpGet]
    [Route("AccountBasicInfo")]
    public ActionResult<AccountBasicInfo> GetAccountBasicInfo(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetAccountBasicInfo),
            new AccountBasicInfo(account.qc.AccountEquity, account.qc.AccountFreeMargin, account.qc.AccountProfit)
        );
    }

    [HttpGet]
    [Route("QuoteClient")]
    public ActionResult<QuoteClient> GetQuoteClient(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetQuoteClient),
            JsonSerializer.Serialize(
                account.qc,
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("OpenedOrders")]
    public ActionResult<Order[]> GetOpenedOrders(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetOpenedOrders),
            JsonSerializer.Serialize(
                account.qc.GetOpenedOrders(),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("Symbols")]
    public ActionResult<SymbolInfo[]> GetSymbols(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetSymbols),
            JsonSerializer.Serialize(
                account.qc.SymbolsInfo,
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("SymbolParams")]
    public ActionResult<SymbolInfo> GetSymbolInfo(ulong id, string symbol)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetSymbolInfo),
            JsonSerializer.Serialize(
                account.qc.GetSymbolInfo(symbol),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("OpenedOrder")]
    public ActionResult<Order> GetOpenedOrder(ulong id, int ticket)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetOpenedOrder),
            JsonSerializer.Serialize(
                account.qc.GetOpenedOrder(ticket),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("OrderHistory")]
    public ActionResult<Order[]> GetOrderHistory(ulong id, string from, string to)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        DateTime start = DateTime.ParseExact(from, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
        DateTime end = DateTime.ParseExact(to, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

        return CreatedAtAction(
            nameof(GetOrderHistory),
            JsonSerializer.Serialize(
                account.qc.DownloadOrderHistory(start, end),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("QuoteHistory")]
    public ActionResult<Bar[]> GetQuoteHistory(ulong id, string symbol, string timeframe, string from, int count)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        Timeframe tf = 0;
        switch (timeframe)
        {
            case "M1":
                tf = Timeframe.M1;
                break;
            case "M5":
                tf = Timeframe.M5;
                break;
            case "M15":
                tf = Timeframe.M15;
                break;
            case "M30":
                tf = Timeframe.M30;
                break;
            case "H1":
                tf = Timeframe.H1;
                break;
            case "H4":
                tf = Timeframe.H4;
                break;
            case "D1":
                tf = Timeframe.D1;
                break;
            case "W1":
                tf = Timeframe.W1;
                break;
            case "MN1":
                tf = Timeframe.MN1;
                break;
            default:
                StatusCode(400);
                break;
        }

        DateTime start = DateTime.ParseExact(from, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

        return CreatedAtAction(
            nameof(GetQuoteHistory),
            JsonSerializer.Serialize(
                account.qc.DownloadQuoteHistory(symbol, tf, start, count),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("OrderSend")]
    public ActionResult<Order> GetOrderSend(ulong id, string symbol, string operation,
                                            double volume, double? price, int? slippage,
                                            double? stoploss, double? takeprofit, string? comment,
                                            int? magic, string? expiration)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        Op op = 0;
        switch (operation)
        {
            case "Buy":
                op = Op.Buy;
                break;
            case "Sell":
                op = Op.Sell;
                break;
            case "BuyLimit":
                op = Op.BuyLimit;
                break;
            case "SellLimit":
                op = Op.SellLimit;
                break;
            case "BuyStop":
                op = Op.BuyStop;
                break;
            case "SellStop":
                op = Op.SellStop;
                break;
            default:
                StatusCode(400);
                break;
        }

        DateTime e;
        if (expiration is null)
            e = default(DateTime);
        else
            e = DateTime.ParseExact(expiration, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

        return CreatedAtAction(
            nameof(GetOrderSend),
            JsonSerializer.Serialize(
                account.oc.OrderSend(
                    symbol, op, volume,
                    price ?? account.qc.GetQuote(symbol).Bid,
                    slippage ?? 200,
                    stoploss ?? 0,
                    takeprofit ?? 0,
                    comment ?? string.Empty,
                    magic ?? 0,
                    e
                ),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("OrderModify")]
    public ActionResult<Order> GetOrderModify(ulong id, int ticket, double stoploss, double takeprofit, double? price, string? expiration)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        DateTime e;
        if (expiration is null)
            e = default(DateTime);
        else
            e = DateTime.ParseExact(expiration, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);

        Order order;
        try
        {
            order = account.qc.GetOpenedOrder(ticket);
        }
        catch (Exception _e)
        {
            Console.WriteLine(_e.ToString());

            return NotFound();
        }

        return CreatedAtAction(
            nameof(GetOrderModify),
            JsonSerializer.Serialize(
                account.oc.OrderModify(
                    ticket,
                    order.Symbol,
                    order.Type,
                    order.Lots,
                    price ?? order.OpenPrice,
                    stoploss,
                    takeprofit,
                    e
                ),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("OrderClose")]
    public ActionResult<Order> GetOrderClose(ulong id, int ticket, double? lots, double? price, int? slippage)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        Order order;
        try
        {
            order = account.qc.GetOpenedOrder(ticket);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return NotFound();
        }

        return CreatedAtAction(
            nameof(GetOrderClose),
            JsonSerializer.Serialize(
                account.oc.OrderClose(
                    order.Symbol,
                    order.Ticket,
                    lots ?? order.Lots,
                    price ?? account.qc.GetQuote(order.Symbol).Bid,
                    slippage ?? 200
                ),
                JsonSerializerOptionsService.converter
            )
        );
    }

    [HttpGet]
    [Route("Disconnect")]
    public IActionResult GetDisconnect(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        if (account.qc.Connected)
        {
            Console.WriteLine("Disconnecting...");
            account.qc.Disconnect();
        }
        AccountService.Delete(id);

        return Ok();
    }
}
