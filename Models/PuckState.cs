namespace nm_be_web_games.Models;

public class PuckState
{
    public Coordinate coordinate { get; } = new Coordinate();
    public Velocity velocity { get; } = new Velocity();
    public PuckState()
    {
    }
}
