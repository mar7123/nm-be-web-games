using System;

namespace nm_be_web_games.Models;

public class Velocity
{
    public float Vx { get; private set; } = 0;
    public float Vy { get; private set; } = 0;

    public Velocity() { }
    public void SetVx(float Vx)
    {
        this.Vx = Vx;
    }
    public void SetVy(float Vy)
    {
        this.Vy = Vy;
    }
}
