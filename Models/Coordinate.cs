using System;

namespace nm_be_web_games.Models;

public class Coordinate
{
    public float X { get; private set; } = 0;
    public float Y { get; private set; } = 0;
    public Coordinate() { }
    public void SetX(float X)
    {
        this.X = X;
    }
    public void SetY(float Y)
    {
        this.Y = Y;
    }
}
