using System.Text.Json.Serialization;

namespace nm_be_web_games.Models.AirHockey;

public class PaddleState : PuckState
{
    public string id { get; private set; }
    public PaddleState(string id)
    {
        this.id = id;
    }
    public void CalculateCoordinateVelocity(Vector2 newCoordinate, int dt)
    {
        Vector2 delta = newCoordinate - coordinate;
        coordinate.SetX(newCoordinate.x);
        coordinate.SetY(newCoordinate.y);
        velocity.SetX(delta.x / dt);
        velocity.SetY(delta.y / dt);

    }
    [JsonConstructor]
    public PaddleState(string id, Vector2 coordinate, Vector2 velocity) : base(coordinate, velocity)
    {
        this.id = id;
    }
}
