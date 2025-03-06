using System;

namespace nm_be_web_games.Models;

public class PaddleState
{
    public string Id { get; private set; }
    public Coordinate Coordinate { get; } = new Coordinate();

    public PaddleState(string Id)
    {
        this.Id = Id;
    }
}
