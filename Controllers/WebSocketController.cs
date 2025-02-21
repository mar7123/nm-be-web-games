using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using nm_be_web_games.Models;
using Serilog;

namespace nm_be_web_games.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private static ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();
        [Route("/ws")]
        public async Task WSRegister()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                string clientId = Guid.NewGuid().ToString();

                _clients.TryAdd(clientId, socket);
                Log.Logger.Information($"Client {clientId} connected.");

                await HandleWebSocketRequest(socket, clientId);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task HandleWebSocketRequest(WebSocket webSocket, string clientId)
        {
            var buffer = new byte[1024 * 4];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    try
                    {
                        GameState? state = JsonSerializer.Deserialize<GameState>(message);
                        if (state != null)
                        {
                            Log.Logger.Information($"Result {message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error($"Error Deserializing\n{ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Error with client {clientId}: {ex.Message}");
            }
            finally
            {
                _clients.TryRemove(clientId, out _);
                if (webSocket.State != WebSocketState.Closed)
                {
                    try
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                    }
                    catch
                    {
                        // pass
                    }
                    finally
                    {
                        try
                        {
                            webSocket.Dispose();
                        }
                        catch
                        {
                            // pass
                        }
                    }
                }
                Log.Logger.Information($"Client {clientId} disconnected.");
            }
        }

    }
}
