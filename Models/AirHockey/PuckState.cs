using System.Text.Json.Serialization;

namespace nm_be_web_games.Models.AirHockey;

public class PuckState
{
    public Vector2 coordinate { get; private set; } = new Vector2();
    public Vector2 velocity { get; private set; } = new Vector2();
    public PuckState()
    {
    }
    [JsonConstructor]
    public PuckState(Vector2 coordinate, Vector2 velocity)
    {
        this.coordinate = coordinate;
        this.velocity = velocity;
    }
}
