using System.Text.Json.Serialization;
using nm_be_web_games.Models.AirHockey;

namespace nm_be_web_games.Models.WSMessage;

public class WSMessageInit : WSMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override WsMessageType type { get; set; } = WsMessageType.INIT_CONFIG;
    public required AirHockeyGameConfig config { get; set; }
}
