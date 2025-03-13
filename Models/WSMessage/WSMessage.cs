namespace nm_be_web_games.Models.WSMessage;

public enum WsMessageType { INIT_CONFIG, GAME_STATE }

public abstract class WSMessage
{
    public abstract WsMessageType type { get; set; }
}
