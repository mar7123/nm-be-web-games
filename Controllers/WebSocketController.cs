using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using nm_be_web_games.Models;
using nm_be_web_games.Models.AirHockey;
using nm_be_web_games.Models.Task;
using nm_be_web_games.Models.WSMessage;
using nm_be_web_games.Repositories;
using nm_be_web_games.Services;
using Serilog;

namespace nm_be_web_games.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private static readonly int _tickRate = 60;
        private static readonly int _tickInterval = 1000 / _tickRate;
        private readonly WebSocketRepository _webSocketRepository;
        private readonly GameStateRepository _gameStateRepository;
        private readonly TaskManager _taskManager;

        public WebSocketController(WebSocketRepository webSocketRepository, GameStateRepository gameStateRepository, TaskManager taskManager)
        {
            _webSocketRepository = webSocketRepository;
            _gameStateRepository = gameStateRepository;
            _taskManager = taskManager;
        }

        [Route("/ws")]
        public async Task<IResult?> WSRegister()
        {
            bool socketReturnHandled = false;
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var roomIdParam = HttpContext.Request.Query["roomId"];
                var playerIdParam = HttpContext.Request.Query["playerId"];
                if (!string.IsNullOrEmpty(roomIdParam) && !string.IsNullOrEmpty(playerIdParam))
                {
                    string roomId = roomIdParam.ToString();
                    string playerId = playerIdParam.ToString();
                    AirHockeyGameState? state = _gameStateRepository.GetGameState(roomId);
                    if (state != null)
                    {
                        PaddleState? paddleState = state.GetPaddleState(playerId);
                        if (paddleState == null)
                        {
                            paddleState = new PaddleState(playerId);
                            Vector2 paddleCoordinate = state.config.GetInitPlayer2Coordinate();
                            paddleState.SetCoordinate(paddleCoordinate);
                            bool registerNewPaddleSuccess = false;
                            await _gameStateRepository.UpdateGameState(roomId, (currentState) =>
                              {
                                  registerNewPaddleSuccess = currentState.RegisterNewPaddle(paddleState);
                                  return Task.CompletedTask;
                              });
                            if (!registerNewPaddleSuccess)
                            {
                                return Results.BadRequest();
                            }
                        }
                        Log.Logger.Information($"room {roomId} continued.");
                        WebSocket? webSocket = _webSocketRepository.GetWebSocket(paddleState.id);
                        if (webSocket == null)
                        {
                            webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                            bool addWebSocketSuccess = _webSocketRepository.AddWebSocket(playerId, webSocket);
                            if (!addWebSocketSuccess)
                            {
                                webSocket.Dispose();
                                return Results.BadRequest();
                            }
                        }
                        await SendGameConfig(webSocket, state);
                        Log.Logger.Information($"Client {paddleState.id} connected.");
                        if (!_taskManager.IsTaskRunning(roomId))
                        {
                            var broadCastFunc = RunBroadcastLoop;
                            var parameters = new GameStateTaskParameters { Id = roomId, Cts = new CancellationTokenSource(), StateId = roomId };
                            bool addTaskSuccess = _taskManager.StartTask(broadCastFunc, parameters);
                        }
                        socketReturnHandled = await HandleWebSocketRequest(webSocket, state.id, paddleState.id);
                    }
                    else
                    {
                        Log.Logger.Information($"room {roomId} started.");
                        AirHockeyGameConfig config = new AirHockeyGameConfig
                        {
                            tickInterval = _tickInterval,
                        };
                        AirHockeyGameState newState = new AirHockeyGameState(roomId, config);
                        Vector2 initPuckCoordinate = config.GetInitPuckCoordinate();
                        newState.puck.SetCoordinate(initPuckCoordinate);
                        newState.SetPaddle1(new PaddleState(playerId));
                        if (newState.paddle1 == null) return Results.BadRequest();
                        bool addStateSuccess = _gameStateRepository.AddGameState(roomId, newState);
                        if (!addStateSuccess)
                        {
                            return Results.BadRequest();
                        }
                        using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                        bool addWebSocketSuccess = _webSocketRepository.AddWebSocket(playerId, webSocket);
                        if (!addWebSocketSuccess)
                        {
                            _gameStateRepository.RemoveGameState(roomId);
                            return Results.BadRequest();
                        }
                        await SendGameConfig(webSocket, newState);
                        var broadCastFunc = RunBroadcastLoop;
                        var parameters = new GameStateTaskParameters { Id = roomId, Cts = new CancellationTokenSource(), StateId = roomId };
                        bool addTaskSuccess = _taskManager.StartTask(broadCastFunc, parameters);
                        if (!addTaskSuccess)
                        {
                            _gameStateRepository.RemoveGameState(roomId);
                            _webSocketRepository.RemoveWebSocket(playerId);
                            return Results.BadRequest();
                        }
                        socketReturnHandled = await HandleWebSocketRequest(webSocket, newState.id, newState.paddle1.id);
                    }
                }
                else
                {
                    return Results.BadRequest();
                }
            }
            return socketReturnHandled ? null : Results.BadRequest();
        }

        private async Task SendGameConfig(WebSocket webSocket, AirHockeyGameState state)
        {

            string gameConfigJson = JsonSerializer.Serialize(new WSMessageInit { config = state.config });
            byte[] message = Encoding.UTF8.GetBytes(gameConfigJson);
            await webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task<bool> HandleWebSocketRequest(WebSocket webSocket, string stateId, string paddleStateId)
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
                        PaddleState? newPaddleState = JsonSerializer.Deserialize<PaddleState>(message);
                        if (newPaddleState != null)
                        {
                            AirHockeyGameState? state = _gameStateRepository.GetGameState(stateId);
                            if (state != null)
                            {
                                PaddleState? paddleState = state.GetPaddleState(paddleStateId);
                                paddleState?.UpdateCoordinate(newPaddleState.coordinate);
                            }
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
                Log.Logger.Error($"Error with client {paddleStateId}: {ex.Message}");
            }
            finally
            {
                _webSocketRepository.RemoveWebSocket(paddleStateId);
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
                    webSocket.Dispose();
                }
                Log.Logger.Information($"Client {paddleStateId} disconnected.");
            }
            return true;
        }
        private async Task RunBroadcastLoop(BaseTaskParameters baseParameters)
        {
            if (baseParameters is GameStateTaskParameters parameters)
            {
                try
                {
                    while (!parameters.Cts.Token.IsCancellationRequested)
                    {
                        AirHockeyGameState? state = _gameStateRepository.GetGameState(parameters.StateId);
                        if (state == null) break;
                        state.CalculateState();
                        var paddle1 = state.paddle1;
                        var paddle2 = state.paddle2;
                        WebSocket? webSocket1 = paddle1 != null ? _webSocketRepository.GetWebSocket(paddle1.id) : null;
                        WebSocket? webSocket2 = paddle2 != null ? _webSocketRepository.GetWebSocket(paddle2.id) : null;
                        if (webSocket1?.State != WebSocketState.Open && webSocket2?.State != WebSocketState.Open) break;
                        string gameStateJson = JsonSerializer.Serialize(new WSMessageGameState { state = state });
                        byte[] message = Encoding.UTF8.GetBytes(gameStateJson);

                        List<Task> tasks = new List<Task>(2);
                        if (webSocket1 != null)
                        {
                            tasks.Append(webSocket1.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None));
                        }
                        if (webSocket2 != null)
                        {
                            tasks.Append(webSocket2.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None));
                        }
                        await Task.WhenAll(tasks);

                        await Task.Delay(_tickInterval);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in RunBroadcastLoop: {ex.Message}");
                }
                finally
                {
                    _gameStateRepository.RemoveGameState(parameters.StateId);
                }
            }
        }
    }
}
