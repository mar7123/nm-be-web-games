using System.Text.Json.Serialization;

namespace nm_be_web_games.Models.AirHockey;

public class PaddleState : PuckState
{
    public string id { get; private set; }
    public PaddleState(string id)
    {
        this.id = id;
    }
    [JsonConstructor]
    public PaddleState(string id, Vector2 coordinate) : base(coordinate)
    {
        this.id = id;
    }
}
