using MT4REST.Models;
using MT4REST.Services;
using TradingAPI.MT4Server;
using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace MT4REST.Controllers;

[ApiController]
[Route("MT4")]
public class WebSocketController : ControllerBase
{
    private readonly ILogger<WebSocketController> _logger;
    private Account? account;

    public WebSocketController(ILogger<WebSocketController> logger)
    {
        _logger = logger;
    }

    private async Task SendQuote(HttpContext context, WebSocket webSocket)
    {
        async void qc_OnQuote(object sender, QuoteEventArgs args)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(args, JsonSerializerOptionsService.converter))),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }

        if (account is null)
            await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "account should not null here.", CancellationToken.None);
        else
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes("QuoteEvent will be sending to this channel.")),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            account.qc.OnQuote += new QuoteEventHandler(qc_OnQuote);

            // 通过从客户端读取数据来检测客户端是否断开
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }

    [HttpGet]
    [Route("Events")]
    public async Task Get(ulong id)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            account = AccountService.Get(id);
            if (account is null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await SendQuote(HttpContext, webSocket);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
