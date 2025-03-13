using System.Text.Json.Serialization;

namespace nm_be_web_games.Models.AirHockey;

public enum AirHockeyPlayerType { PLAYER_1, PLAYER_2 }

public class AirHockeyGameConfig
{
    public required String roomId { get; set; }
    public required String playerId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AirHockeyPlayerType playerType { get; set; }
    public int tickrate { get; set; }
    public float airHockeyTableWidth { get; set; } = 500;
    public float airHockeyTableHeight { get; set; } = 1000;
    public float paddleSize { get; set; } = 80;
    public float puckSize { get; set; } = 50;
    public float goalWidthScale { get; set; } = 0.5F;
}
