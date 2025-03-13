using System.Collections.Concurrent;
using nm_be_web_games.Models.Task;
using Serilog;

namespace nm_be_web_games.Services;

public class TaskManager
{
    private readonly ConcurrentDictionary<string, (Task task, CancellationTokenSource cts)> _tasks = new ConcurrentDictionary<string, (Task task, CancellationTokenSource cts)>();

    public TaskManager()
    {
    }
    public bool StartTask(Func<BaseTaskParameters, Task> taskFunction, BaseTaskParameters parameters)
    {
        string taskId = parameters.Id;
        if (_tasks.ContainsKey(taskId))
        {
            Log.Logger.Information($"Task with ID {taskId} is already running.");
            return false;
        }

        var task = Task.Run(async () =>
        {
            try
            {
                Log.Logger.Information($"Task with ID {taskId}");
                await taskFunction(parameters);
            }
            catch (OperationCanceledException)
            {
                Log.Logger.Error($"Task {taskId} was canceled.");
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Task {taskId} failed: {ex.Message}");
            }
            finally
            {
                if (_tasks.TryRemove(taskId, out var entry))
                {
                    entry.cts.Dispose();
                }
                Log.Logger.Information($"Task {taskId} completed and cleaned up.");
            }
        }, parameters.Cts.Token);

        if (task.IsCompleted) return true;
        if (_tasks.TryAdd(taskId, (task, parameters.Cts)))
        {
            Log.Logger.Information($"Task {taskId} started.");
            return true;
        }

        return false;
    }

    public bool CancelTask(string taskId)
    {
        if (_tasks.TryRemove(taskId, out var taskEntry))
        {
            taskEntry.cts.Cancel();
            Log.Logger.Information($"Task {taskId} cancelled.");
            taskEntry.cts.Dispose();
            return true;
        }

        Log.Logger.Information($"Task {taskId} not found.");
        return false;
    }

    public bool IsTaskRunning(string taskId)
    {
        return _tasks.ContainsKey(taskId);
    }

    public async Task WaitForTaskCompletion(string taskId)
    {
        if (_tasks.TryGetValue(taskId, out var taskEntry))
        {
            await taskEntry.task;
        }
    }
}
