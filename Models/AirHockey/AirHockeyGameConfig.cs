using System.Text.Json.Serialization;

namespace nm_be_web_games.Models.AirHockey;

public enum AirHockeyPlayerType { PLAYER_1, PLAYER_2 }

public class AirHockeyGameConfig
{
    public required int tickInterval { get; set; }
    public float puckResistance { get; set; } = 0.99f;
    public float airHockeyTableWidth { get; set; } = 500;
    public float airHockeyTableHeight { get; set; } = 1000;
    public float paddleSize { get; set; } = 80;
    public float puckSize { get; set; } = 50;
    public float goalWidthScale { get; set; } = 0.5F;

    public Vector2 GetInitPuckCoordinate()
    {
        return new Vector2(airHockeyTableWidth / 2, airHockeyTableHeight / 2);
    }
    public Vector2 GetInitPlayer2Coordinate()
    {
        return new Vector2(airHockeyTableWidth / 2, airHockeyTableHeight / 4);
    }
    public float GetPaddlePuckIntersectLength()
    {
        return (paddleSize + puckSize) / 2;
    }
}
