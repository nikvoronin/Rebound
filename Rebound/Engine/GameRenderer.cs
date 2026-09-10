using System.Windows.Controls;

using Rebound.Controls;
using Rebound.Models;

namespace Rebound.Engine;

internal sealed class GameRenderer(Canvas canvas)
{
    public void Clear()
    {
        _canvas.Children.Clear();
        _brickVisuals.Clear();

        _paddleVisual = null;
        _ballVisual = null;
    }

    public void RenderBricks(IReadOnlyList<Brick> bricks)
    {
        foreach (Brick brick in bricks)
        {
            var visual = new BrickVisual(brick.Bounds, brick.Color, brick.HitsRemaining);

            Canvas.SetLeft(visual, brick.Bounds.X);
            Canvas.SetTop(visual, brick.Bounds.Y);

            _canvas.Children.Add(visual);
            _brickVisuals[brick] = visual;
        }
    }

    public void UpdateBrickVisual(Brick brick)
    {
        if (_brickVisuals.TryGetValue(brick, out BrickVisual? visual))
            visual.SetAppearance(brick.Bounds, brick.Color, brick.HitsRemaining);
    }

    public void RemoveBrickVisual(Brick brick)
    {
        if (_brickVisuals.Remove(brick, out BrickVisual? visual))
            _canvas.Children.Remove(visual);
    }

    public void CreatePaddleAndBallVisuals(Paddle paddle, Ball ball)
    {
        _ballVisual = new BallVisual(ball.Radius);
        _canvas.Children.Add(_ballVisual);

        _paddleVisual = new PaddleVisual(paddle.Width, paddle.Height);
        Canvas.SetLeft(_paddleVisual, paddle.X);
        Canvas.SetTop(_paddleVisual, paddle.Y);
        _canvas.Children.Add(_paddleVisual);
    }

    public void UpdatePositions(Paddle paddle, Ball ball)
    {
        if (_paddleVisual is not null)
        {
            Canvas.SetLeft(_paddleVisual, paddle.X);
            Canvas.SetTop(_paddleVisual, paddle.Y);
        }

        if (_ballVisual is not null)
        {
            Canvas.SetLeft(_ballVisual, ball.X - ball.Radius);
            Canvas.SetTop(_ballVisual, ball.Y - ball.Radius);
        }
    }

    private readonly Canvas _canvas = canvas;
    private readonly Dictionary<Brick, BrickVisual> _brickVisuals = [];

    private PaddleVisual? _paddleVisual;
    private BallVisual? _ballVisual;
}
