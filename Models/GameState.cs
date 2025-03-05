using System;

namespace nm_be_web_games.Models;

public class GameState
{
    public required string Id { get; set; }
    public float PuckX { get; set; }
    public float PuckY { get; set; }
    public float PuckVX { get; set; } = 0;
    public float PuckVY { get; set; } = 0;
    public PaddleState? Paddle1 { get; set; }
    public PaddleState? Paddle2 { get; set; }
    public int Score1 { get; set; } = 0;
    public int Score2 { get; set; } = 0;

    public PaddleState? GetPaddleState(String Id)
    {
        if (Paddle1 != null && Paddle1.Id == Id)
        {
            return Paddle1;
        }
        if (Paddle2 != null && Paddle2.Id == Id)
        {
            return Paddle2;
        }
        return null;
    }

    public bool IsPlayerFull()
    {
        return Paddle1 != null && Paddle2 != null;
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
