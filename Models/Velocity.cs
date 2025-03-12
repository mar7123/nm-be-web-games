using System.Text.Json.Serialization;

namespace nm_be_web_games.Models;

public class Velocity
{
    public float vX { get; private set; } = 0;
    public float vY { get; private set; } = 0;

    public Velocity() { }
    public void SetVX(float vX)
    {
        this.vX = vX;
    }
    public void SetVY(float vY)
    {
        this.vY = vY;
    }

    [JsonConstructor]
    public Velocity(float vX, float vY)
    {
        this.vX = vX;
        this.vY = vY;
    }
}
