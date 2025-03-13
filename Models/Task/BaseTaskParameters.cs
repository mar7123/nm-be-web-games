namespace nm_be_web_games.Models.Task;

public class BaseTaskParameters
{
    public required string Id { get; set; }
    public required CancellationTokenSource Cts { get; set; }
}
