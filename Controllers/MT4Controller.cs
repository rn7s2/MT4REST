using MT4REST.Services;
using TradingAPI.MT4Server;
using Microsoft.AspNetCore.Mvc;

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
            Console.WriteLine("Server build: {}", qc.ServerBuild);
            while (qc.ServerTime == DateTime.MinValue)
                Thread.Sleep(10);
            Console.WriteLine("Server time: {}", qc.ServerTime);

            return CreatedAtAction(nameof(GetConnect), AccountService.Add(qc));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());

            return StatusCode(403);
        }
    }

    [HttpGet]
    [Route("Groups")]
    public ActionResult<ConSymbolGroup[]> GetGroups(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();
        return CreatedAtAction(nameof(GetGroups), account.qc.Groups);
    }

    [HttpGet]
    [Route("QuoteClient")]
    public ActionResult<QuoteClient> GetQuoteClient(ulong id)
    {
        var account = AccountService.Get(id);
        if (account is null)
            return NotFound();

        return CreatedAtAction(nameof(GetQuoteClient), account.qc);
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
