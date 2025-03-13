using System.Text.Json.Serialization;
using nm_be_web_games.Models.AirHockey;

namespace nm_be_web_games.Models.WSMessage;

public class WSMessageGameState : WSMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override WsMessageType type { get; set; } = WsMessageType.GAME_STATE;
    public required AirHockeyGameState state { get; set; }
}
