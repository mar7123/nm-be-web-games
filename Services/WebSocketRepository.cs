using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace nm_be_web_games.Services;

public class WebSocketRepository
{
    private readonly ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();

    public WebSocketRepository()
    {
    }

    public WebSocket? GetWebSocket(string clientId)
    {
        _clients.TryGetValue(clientId, out var socket);
        return socket;
    }

    public bool AddWebSocket(string clientId, WebSocket webSocket)
    {
        return _clients.TryAdd(clientId, webSocket);
    }
    public bool RemoveWebSocket(string clientId)
    {
        return _clients.TryRemove(clientId, out _);
    }
}
