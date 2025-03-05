using System;

namespace nm_be_web_games.Models;

public class PaddleState
{
    public required string Id { get; set; }
    public float PaddleX { get; set; }
    public float PaddleY { get; set; }
}
