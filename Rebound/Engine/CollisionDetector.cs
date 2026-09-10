using System.Windows;

using Rebound.Models;

namespace Rebound.Engine;

internal static class CollisionDetector
{
    public static bool CircleIntersectsRect(
        double x, 
        double y, 
        double radius, 
        Rect rect)
    {
        double closestX = Math.Clamp(x, rect.Left, rect.Right);
        double closestY = Math.Clamp(y, rect.Top, rect.Bottom);

        double dx = x - closestX;
        double dy = y - closestY;

        return dx * dx + dy * dy <= radius * radius;
    }

    public static bool BallHitsPaddle(Ball ball, Paddle paddle)
    {
        // Only collide when ball is moving downward.
        if (ball.VY <= 0.0)
            return false;

        double batTop = paddle.Y;
        double batBottom = paddle.Y + paddle.Height;

        if (ball.Y + ball.Radius < batTop 
            || ball.Y - ball.Radius > batBottom)
        { 
            return false; 
        }

        if (ball.X + ball.Radius < paddle.X 
            || ball.X - ball.Radius > paddle.X + paddle.Width)
        {
            return false;
        }

        return true;
    }

    public static Brick? FindHitBrick(Ball ball, IReadOnlyList<Brick> bricks)
    {
        for (int i = bricks.Count - 1; i >= 0; i--)
        {
            Brick brick = bricks[i];

            if (CircleIntersectsRect(ball.X, ball.Y, ball.Radius, brick.Bounds))
                return brick;
        }

        return null;
    }

    public static void ResolveBrickCollision(Ball ball, Rect rect)
    {
        double closestX = Math.Clamp(ball.X, rect.Left, rect.Right);
        double closestY = Math.Clamp(ball.Y, rect.Top, rect.Bottom);

        double dx = ball.X - closestX;
        double dy = ball.Y - closestY;
        double distance = Math.Sqrt(dx * dx + dy * dy);

        // Rare fallback if ball center is exactly inside brick bounds.
        if (distance < 0.0001)
        {
            ball.VY = -Math.Abs(ball.VY);
            ball.Y = rect.Top - ball.Radius;
            return;
        }

        double overlap = ball.Radius - distance;
        if (overlap > 0.0)
        {
            ball.X += dx / distance * overlap;
            ball.Y += dy / distance * overlap;
        }

        if (Math.Abs(dx) > Math.Abs(dy))
        {
            ball.VX = dx >= 0.0 
                ? Math.Abs(ball.VX) 
                : -Math.Abs(ball.VX);
        }
        else
        {
            ball.VY = dy >= 0.0 
                ? Math.Abs(ball.VY) 
                : -Math.Abs(ball.VY);
        }
    }
}
