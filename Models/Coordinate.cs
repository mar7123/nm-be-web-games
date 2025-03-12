using System.Text.Json.Serialization;

namespace nm_be_web_games.Models;

public class Coordinate
{
    public float x { get; private set; } = 0;
    public float y { get; private set; } = 0;
    public Coordinate() { }
    public void SetX(float x)
    {
        this.x = x;
    }
    public void SetY(float y)
    {
        this.y = y;
    }

    [JsonConstructor]
    public Coordinate(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
