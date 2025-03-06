using System;

namespace nm_be_web_games.Models;

public class PuckState
{
    public Coordinate Coordinate { get; } = new Coordinate();
    public Velocity Velocity { get; } = new Velocity();
    public PuckState()
    {
    }
}
