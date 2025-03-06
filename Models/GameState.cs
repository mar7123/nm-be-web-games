using System;

namespace nm_be_web_games.Models;

public class GameState
{
    public string Id { get; private set; }
    public PuckState Puck { get; } = new PuckState();
    public PaddleState? Paddle1 { get; private set; }
    public PaddleState? Paddle2 { get; private set; }
    public int Score1 { get; private set; } = 0;
    public int Score2 { get; private set; } = 0;

    public GameState(string Id)
    {
        this.Id = Id;
    }

    public void SetPaddle1(PaddleState paddle)
    {
        Paddle1 = paddle;
    }
    public void SetPaddle2(PaddleState paddle)
    {
        Paddle2 = paddle;
    }
    public PaddleState? GetPaddleState(string paddleId)
    {
        if (Paddle1 != null && Paddle1.Id == paddleId)
        {
            return Paddle1;
        }
        if (Paddle2 != null && Paddle2.Id == paddleId)
        {
            return Paddle2;
        }
        return null;
    }
    public void RegisterNewPaddle(PaddleState newPaddle)
    {
        if (Paddle1 == null)
        {
            Paddle1 = newPaddle;
        }
        else if (Paddle2 == null)
        {
            Paddle2 = newPaddle;
        }
    }
}
