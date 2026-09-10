using System.Windows;

namespace Rebound.Models;

internal sealed class Ball(double radius)
{
    public double X { get; set; }
    public double Y { get; set; }
    public double VX { get; set; }
    public double VY { get; set; }
    public double Radius { get; } = radius;
    public bool IsAttached { get; set; } = true;

    public Rect Bounds => new(
        X - Radius, 
        Y - Radius, 
        Radius * 2.0, 
        Radius * 2.0);

    public void AttachToPaddle(Paddle paddle)
    {
        X = paddle.CenterX;
        Y = paddle.Y - Radius - 2.0;

        VX = 0.0;
        VY = 0.0;
    }

    public void Launch(double angle, double speed)
    {
        IsAttached = false;

        VX = speed * Math.Sin(angle);
        VY = -speed * Math.Cos(angle);
    }

    /// <summary>
    /// Integrates position and bounces off the left/right/top walls. 
    /// Returns true if the ball fell past the bottom edge.
    /// </summary>
    public bool MoveAndBounceOffWalls(
        double dt, 
        double fieldWidth, 
        double fieldHeight)
    {
        X += VX * dt;
        Y += VY * dt;

        // Left wall.
        if (X - Radius < 0.0)
        {
            X = Radius;
            VX = Math.Abs(VX);
        }

        // Right wall.
        if (X + Radius > fieldWidth)
        {
            X = fieldWidth - Radius;
            VX = -Math.Abs(VX);
        }

        // Top wall.
        if (Y - Radius < 0.0)
        {
            Y = Radius;
            VY = Math.Abs(VY);
        }

        // Bottom: lose life.
        return Y - Radius > fieldHeight;
    }
}
