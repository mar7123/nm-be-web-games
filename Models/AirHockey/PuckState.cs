using System.Text.Json.Serialization;

namespace nm_be_web_games.Models.AirHockey;

public class PuckState
{
    public Vector2 coordinate { get; private set; } = new Vector2();
    [JsonIgnore]
    public Vector2 oldCoordinate { get; private set; } = new Vector2();
    [JsonIgnore]
    public Vector2 velocity { get; private set; } = new Vector2();
    public PuckState()
    {
    }
    [JsonConstructor]
    public PuckState(Vector2 coordinate)
    {
        this.coordinate = coordinate;
    }
    public void SetCoordinate(Vector2 newCoordinate)
    {
        oldCoordinate.UpdateValue(newCoordinate);
        coordinate.UpdateValue(newCoordinate);
    }
    public void UpdateCoordinate(Vector2 newCoordinate)
    {
        oldCoordinate.UpdateValue(coordinate);
        coordinate.UpdateValue(newCoordinate);
    }
    public void UpdateVelocity(int dt)
    {
        Vector2 delta = coordinate - oldCoordinate;
        velocity.UpdateValue(new Vector2(delta.x / dt, delta.y / dt));
    }
}
