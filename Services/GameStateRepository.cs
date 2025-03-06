using System;
using System.Collections.Concurrent;
using nm_be_web_games.Models;

namespace nm_be_web_games.Repositories;

public class GameStateRepository
{
    private readonly ConcurrentDictionary<string, GameState> _gameStates = new ConcurrentDictionary<string, GameState>();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
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
        return _gameStates.TryAdd(stateId, state) && _locks.TryAdd(stateId, new SemaphoreSlim(1, 1));
    }
    public async Task<bool> UpdateGameState(string stateId, Func<GameState, Task> update)
    {
        _locks.TryGetValue(stateId, out var stateLock);
        GameState? state = GetGameState(stateId);
        if (state == null || stateLock == null) return false;
        await stateLock.WaitAsync();
        try
        {
            await update(state);
            return true;
        }
        finally
        {
            stateLock.Release();
        }
    }
    public bool RemoveGameState(string stateId)
    {
        return _gameStates.TryRemove(stateId, out _) && _locks.TryRemove(stateId, out _);
    }
}
