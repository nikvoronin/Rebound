namespace Rebound.Engine;

internal static class GameConstants
{
    public const double FieldWidth = 600.0;
    public const double FieldHeight = 700.0;

    public const double BatWidth = 120.0;
    public const double BatHeight = 16.0;
    public const double BatY = FieldHeight - 64.0;

    public const double BallRadius = 9.0;
    public const double BaseBallSpeed = 340.0;
    public const double BatKeyboardSpeed = 520.0;
    public const double MaxBounceAngle = Math.PI / 3.0; // 60 degrees
    public const double MaxBallSpeed = 1100.0;
    public const int PaddleHitsPerSpeedTier = 4;
    public const double SpeedTierIncrement = 40.0;
    public const int MaxSpeedTiers = 3;
    public const double EdgeSlowdownFactor = 0.3;
}
