namespace nm_be_web_games.Models;

public class GameState
{
    public string id { get; private set; }
    public PuckState puck { get; } = new PuckState();
    public PaddleState? paddle1 { get; private set; }
    public PaddleState? paddle2 { get; private set; }
    public int score1 { get; private set; } = 0;
    public int score2 { get; private set; } = 0;

    public GameState(string id)
    {
        this.id = id;
    }

    public void SetPaddle1(PaddleState paddle)
    {
        paddle1 = paddle;
    }
    public void SetPaddle2(PaddleState paddle)
    {
        paddle2 = paddle;
    }
    public PaddleState? GetPaddleState(string paddleId)
    {
        if (paddle1 != null && paddle1.id == paddleId)
        {
            return paddle1;
        }
        if (paddle2 != null && paddle2.id == paddleId)
        {
            return paddle2;
        }
        return null;
    }
    public void RegisterNewPaddle(PaddleState newPaddle)
    {
        if (paddle1 == null)
        {
            paddle1 = newPaddle;
        }
        else if (paddle2 == null)
        {
            paddle2 = newPaddle;
        }
    }
}
