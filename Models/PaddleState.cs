namespace nm_be_web_games.Models;

public class PaddleState : PuckState
{
    public string id { get; private set; }
    public PaddleState(string id)
    {
        this.id = id;
    }
}
