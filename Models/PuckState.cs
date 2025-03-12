using System.Text.Json.Serialization;

namespace nm_be_web_games.Models;

public class PuckState
{
    public Coordinate coordinate { get; private set; } = new Coordinate();
    public Velocity velocity { get; private set; } = new Velocity();
    public PuckState()
    {
    }

    [JsonConstructor]
    public PuckState(Coordinate coordinate, Velocity velocity)
    {
        this.coordinate = coordinate;
        this.velocity = velocity;
    }
}
