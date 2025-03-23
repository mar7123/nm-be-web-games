using System.Numerics;
using System.Text.Json.Serialization;
using Serilog;

namespace nm_be_web_games.Models.AirHockey;

public class AirHockeyGameState
{
    public string id { get; private set; }
    [JsonIgnore]
    public AirHockeyGameConfig config { get; set; }
    public PuckState puck { get; } = new PuckState();
    public PaddleState? paddle1 { get; private set; }
    public PaddleState? paddle2 { get; private set; }
    public int score1 { get; private set; } = 0;
    public int score2 { get; private set; } = 0;

    public AirHockeyGameState(string id, AirHockeyGameConfig config)
    {
        this.id = id;
        this.config = config;
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

    public AirHockeyPlayerType GetAirHockeyPlayerType(string paddleId)
    {
        if (paddle1 != null && paddle1.id == paddleId)
        {
            return AirHockeyPlayerType.PLAYER_1;
        }
        if (paddle2 != null && paddle2.id == paddleId)
        {
            return AirHockeyPlayerType.PLAYER_2;
        }
        return AirHockeyPlayerType.PLAYER_2;
    }
    public bool RegisterNewPaddle(PaddleState newPaddle)
    {
        if (paddle1 == null)
        {
            paddle1 = newPaddle;
            return true;
        }
        else if (paddle2 == null)
        {
            paddle2 = newPaddle;
            return true;
        }
        return false;
    }
    private (Vector2?, Vector2?) GetDistanceRPoints(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, float r)
    {
        Vector2 d1 = A2 - A1;  // Direction vector of object 1
        Vector2 d2 = B2 - B1;  // Direction vector of object 2

        // Quadratic equation coefficients
        float A = d1.SquaredMagnitude() + d2.SquaredMagnitude() - 2 * (d1.x * d2.x + d1.y * d2.y);
        float B = 2 * ((A1.x - B1.x) * (d1.x - d2.x) + (A1.y - B1.y) * (d1.y - d2.y));
        float C = (A1 - B1).SquaredMagnitude() - r * r;

        // Solve for t using quadratic formula
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            return (null, null);  // No valid solutions
        }

        float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
        float t1 = (-B - sqrtDiscriminant) / (2 * A);
        float t2 = (-B + sqrtDiscriminant) / (2 * A);

        float t = -1;
        if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
        {
            t = Math.Min(t1, t2);
        }
        else if (t1 >= 0 && t1 <= 1)
        {
            t = t1;
        }
        else if (t2 >= 0 && t2 <= 1)
        {
            t = t2;
        }
        if (t == -1)
        {
            return (null, null);
        }
        t *= 1.001f;

        return (A1 + d1 * t, B1 + d2 * t);
    }
    private void CalculatePuckImpact(PaddleState paddleState)
    {
        Vector2 diff = puck.coordinate - paddleState.coordinate;
        double distance = diff.Magnitude();
        double collisionDistance = config.GetPaddlePuckIntersectLength();

        if (distance < collisionDistance) // Check for collision
        {
            Vector2 normal = diff.Normalize();  // Get the normal of collision
            Vector2 relativeVelocity = puck.velocity - paddleState.velocity;
            double velocityDot = relativeVelocity.Dot(normal);

            if (velocityDot < 0) // Ensure collision is meaningful
            {
                // Reflect puck velocity correctly
                Vector2 reflectedVelocity = puck.velocity - normal * (2 * velocityDot);

                // Apply paddle influence in a smoother way
                Vector2 paddleInfluence = paddleState.velocity * 0.2; // Reduce factor for stability

                // Update the puck's velocity
                Vector2 newPuckVelocity = reflectedVelocity + paddleInfluence;
                puck.velocity.UpdateValue(newPuckVelocity);

                // Move puck slightly out of collision zone to prevent overlap
                Vector2 newCoordinate = puck.coordinate + (normal * (collisionDistance - distance + 0.1));
                puck.coordinate.UpdateValue(newCoordinate);
            }
        }
    }

    public void CalculateState()
    {
        // Update paddles' velocities first
        if (paddle1 != null)
        {
            paddle1.UpdateVelocity(config.tickInterval);
            CalculatePuckImpact(paddle1);
        }
        if (paddle2 != null)
        {
            paddle2.UpdateVelocity(config.tickInterval);
            CalculatePuckImpact(paddle2);
        }

        // Apply friction properly
        float resistanceFactor = (float)Math.Pow(config.puckResistance, config.tickInterval);
        Vector2 newPuckVelocity = puck.velocity * resistanceFactor;

        // Update puck velocity and position
        puck.velocity.UpdateValue(newPuckVelocity);
        Vector2 newCoordinate = puck.coordinate + (puck.velocity * config.tickInterval);
        puck.UpdateCoordinate(newCoordinate);

        // Boundary collision handling (walls)
        if (puck.coordinate.x - config.puckSize / 2 < 0 || puck.coordinate.x + config.puckSize / 2 > config.airHockeyTableWidth)
        {
            puck.velocity.UpdateValue(new Vector2(-puck.velocity.x * 0.9f, puck.velocity.y)); // Reduce speed slightly on bounce
            puck.coordinate.UpdateValue(new Vector2(
                Math.Clamp(puck.coordinate.x, config.puckSize / 2, config.airHockeyTableWidth - config.puckSize / 2),
                puck.coordinate.y));
        }
        if (puck.coordinate.y - config.puckSize / 2 < 0 || puck.coordinate.y + config.puckSize / 2 > config.airHockeyTableHeight)
        {
            puck.velocity.UpdateValue(new Vector2(puck.velocity.x, -puck.velocity.y * 0.9f)); // Reduce speed slightly on bounce
            puck.coordinate.UpdateValue(new Vector2(
                puck.coordinate.x,
                Math.Clamp(puck.coordinate.y, config.puckSize / 2, config.airHockeyTableHeight - config.puckSize / 2)));
        }
    }
}
