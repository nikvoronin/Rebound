using System.Windows;

using Rebound.Engine;

namespace Rebound.Models;

internal sealed class Paddle(
    double x, 
    double width, 
    double height, 
    double y)
{
    public double X { get; set; } = x;
    public double Width { get; } = width;
    public double Height { get; } = height;
    public double Y { get; } = y;

    public double CenterX => X + Width / 2.0;

    public Rect Bounds => new(X, Y, Width, Height);

    public void HandleKeyboardInput(
        bool leftPressed, 
        bool rightPressed, 
        double dt, 
        double fieldWidth)
    {
        if (leftPressed)
            X -= GameConstants.BatKeyboardSpeed * dt;

        if (rightPressed)
            X += GameConstants.BatKeyboardSpeed * dt;

        Clamp(fieldWidth);
    }

    public void SetXFromPointer(double pointerX, double fieldWidth)
    {
        X = pointerX - Width / 2.0;
        Clamp(fieldWidth);
    }

    public void Clamp(double fieldWidth)
    {
        X = Math.Clamp(X, 0.0, fieldWidth - Width);
    }
}
