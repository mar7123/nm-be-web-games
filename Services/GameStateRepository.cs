using System;
using System.Collections.Concurrent;
using nm_be_web_games.Models;

namespace nm_be_web_games.Repositories;

public class GameStateRepository
{
    private readonly ConcurrentDictionary<string, GameState> _gameStates = new ConcurrentDictionary<string, GameState>();

    public GameStateRepository()
    {
    }

    public GameState? GetGameState(string stateId)
    {
        _gameStates.TryGetValue(stateId, out var state);
        return state;
    }

    public bool AddGameState(string stateId, GameState state)
    {
        return _gameStates.TryAdd(stateId, state);
    }
    public bool UpdateGameState(string stateId, GameState state)
    {
        lock (this)
        {
            GameState? oldState = GetGameState(stateId);
            return oldState != null ? _gameStates.TryUpdate(stateId, state, oldState) : false;
        }
    }
    public bool RemoveGameState(string stateId)
    {
        return _gameStates.TryRemove(stateId, out _);
    }
}
