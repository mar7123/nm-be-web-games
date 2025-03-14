using System.Text.Json.Serialization;

namespace nm_be_web_games.Models;

public class Vector2
{
    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
    public static Vector2 operator *(Vector2 a, double scalar) => new Vector2((float)(a.x * scalar), (float)(a.y * scalar));

    public float x { get; private set; } = 0;
    public float y { get; private set; } = 0;
    [JsonConstructor]
    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector2() { }
    public void SetX(float x)
    {
        this.x = x;
    }
    public void SetY(float y)
    {
        this.y = y;
    }
    public float DistanceTo(Vector2 other)
    {
        return (float)Math.Sqrt(Math.Pow(x - other.x, 2) + Math.Pow(y - other.y, 2));
    }
    public float Dot(Vector2 other)
    {
        return (x * other.x) + (y * other.y);
    }
    public Vector2 Normalize()
    {
        float magnitude = Magnitude();
        if (magnitude == 0)
        {
            return new Vector2(0, 0);
        }
        return new Vector2(x / magnitude, y / magnitude);
    }
    public float Magnitude()
    {
        return (float)Math.Sqrt((x * x) + (y * y));
    }
}
