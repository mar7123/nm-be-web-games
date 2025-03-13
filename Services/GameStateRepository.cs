using System.Collections.Concurrent;
using nm_be_web_games.Models.AirHockey;

namespace nm_be_web_games.Repositories;

public class GameStateRepository
{
    private readonly ConcurrentDictionary<string, AirHockeyGameState> _gameStates = new ConcurrentDictionary<string, AirHockeyGameState>();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    public GameStateRepository()
    {
    }

    public AirHockeyGameState? GetGameState(string stateId)
    {
        _gameStates.TryGetValue(stateId, out var state);
        return state;
    }
    public bool AddGameState(string stateId, AirHockeyGameState state)
    {
        return _gameStates.TryAdd(stateId, state) && _locks.TryAdd(stateId, new SemaphoreSlim(1, 1));
    }
    public async Task<bool> UpdateGameState(string stateId, Func<AirHockeyGameState, Task> update)
    {
        _locks.TryGetValue(stateId, out var stateLock);
        AirHockeyGameState? state = GetGameState(stateId);
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
        bool removeSemaphore = _locks.TryRemove(stateId, out var semaphoreSlim);
        semaphoreSlim?.Dispose();
        return _gameStates.TryRemove(stateId, out _) && removeSemaphore;
    }
}
