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
    private void CalculatePuckImpact(AirHockeyGameConfig config, PaddleState paddleState)
    {
        Vector2 diff = puck.coordinate - paddleState.coordinate;
        double distance = diff.Magnitude();
        double collisionDistance = config.GetPaddlePuckIntersectLength();
        if (distance < collisionDistance)
        {
            Vector2 normal = diff.Normalize();
            Vector2 relativeVelocity = puck.velocity - paddleState.velocity;
            double velocityDot = relativeVelocity.Dot(normal);
            if (velocityDot < 0)
            {
                Vector2 reflectedVelocity = puck.velocity - normal * (2 * velocityDot);
                Vector2 paddleInfluence = paddleState.velocity;
                Vector2 puckSpeed = reflectedVelocity + paddleInfluence;
                puck.velocity.SetX(puckSpeed.x);
                puck.velocity.SetY(puckSpeed.y);
            }
        }
    }

    public void CalculateState()
    {
        if (paddle1 != null)
        {
            CalculatePuckImpact(config, paddle1);
        }
        if (paddle2 != null)
        {
            CalculatePuckImpact(config, paddle2);
        }
        Vector2 newPuckVelocity = puck.velocity * (float)Math.Pow(config.puckResistance, config.tickInterval);
        puck.velocity.SetX(newPuckVelocity.x);
        puck.velocity.SetY(newPuckVelocity.y);
        Vector2 newCoordinate = puck.coordinate + (puck.velocity * config.tickInterval);
        puck.coordinate.SetX(newCoordinate.x);
        puck.coordinate.SetY(newCoordinate.y);

        if (puck.coordinate.x - config.puckSize / 2 < 0 || puck.coordinate.x + config.puckSize / 2 > config.airHockeyTableWidth)
        {
            puck.velocity.SetX(-puck.velocity.x);
            puck.coordinate.SetX(Math.Clamp(puck.coordinate.x, config.puckSize / 2, config.airHockeyTableWidth - config.puckSize / 2));
        }
        if (puck.coordinate.y - config.puckSize / 2 < 0 || puck.coordinate.y + config.puckSize / 2 > config.airHockeyTableHeight)
        {
            puck.velocity.SetY(-puck.velocity.y);
            puck.coordinate.SetY(Math.Clamp(puck.coordinate.y, config.puckSize / 2, config.airHockeyTableHeight - config.puckSize / 2));
        }
    }
}
