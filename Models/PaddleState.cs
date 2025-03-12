using System.Text.Json.Serialization;

namespace nm_be_web_games.Models;

public class PaddleState : PuckState
{
    public string id { get; private set; }
    public PaddleState(string id)
    {
        this.id = id;
    }

    [JsonConstructor]
    public PaddleState(string id, Coordinate coordinate, Velocity velocity) : base(coordinate, velocity)
    {
        this.id = id;
    }
}
