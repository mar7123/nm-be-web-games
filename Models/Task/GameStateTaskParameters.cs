using System;
using System.Net.WebSockets;

namespace nm_be_web_games.Models.Task;

public class GameStateTaskParameters : BaseTaskParameters
{
    public required string StateId { get; set; }

}
