using Rebound.Levels;
using Rebound.Models;

namespace Rebound.Engine;

internal sealed class GameEngine
{
    public GameEngine(GameRenderer renderer, GameHud hud)
    {
        _renderer = renderer;
        _hud = hud;

        _levels = LevelLoader.LoadLevels();

        _paddle = new Paddle(
            (GameConstants.FieldWidth - GameConstants.BatWidth) / 2.0,
            GameConstants.BatWidth,
            GameConstants.BatHeight,
            GameConstants.BatY);

        _ball = new Ball(GameConstants.BallRadius);

        LoadLevel(0);
    }

    public void Update(double dt, bool leftPressed, bool rightPressed)
    {
        if (_state == GameState.Paused)
            return;

        _paddle.HandleKeyboardInput(
            leftPressed, 
            rightPressed, 
            dt, 
            GameConstants.FieldWidth);

        if (_state == GameState.Ready 
            || _state == GameState.Playing)
        {
            if (_ball.IsAttached)
            {
                _ball.AttachToPaddle(_paddle);
            }
            else
            {
                MoveBall(dt);
            }
        }

        _renderer.UpdatePositions(_paddle, _ball);
    }

    public void SetBatPositionFromPointer(double canvasX)
    {
        if (_state == GameState.Paused)
            return;

        _paddle.SetXFromPointer(canvasX, GameConstants.FieldWidth);
    }

    public void TogglePause()
    {
        if (_state == GameState.Paused)
        {
            _state = _stateBeforePause;
            _hud.SetMessage(
                _state == GameState.Ready 
                    ? "PRESS SPACE OR CLICK TO LAUNCH" 
                    : string.Empty);

            return;
        }

        if (_state != GameState.Ready 
            && _state != GameState.Playing)
        {
            return;
        }

        _stateBeforePause = _state;
        _state = GameState.Paused;
        _hud.SetMessage("PAUSED");
    }

    public void PrimaryAction()
    {
        switch (_state)
        {
            case GameState.Ready:
                LaunchBall();
                break;

            case GameState.LevelComplete:
                LoadLevel(_levelIndex + 1);
                break;

            case GameState.Won:
            case GameState.GameOver:
                RestartGame();
                break;
        }
    }

    private void MoveBall(double dt)
    {
        bool fellOffBottom = _ball.MoveAndBounceOffWalls(
            dt, 
            GameConstants.FieldWidth, 
            GameConstants.FieldHeight);

        if (fellOffBottom)
        {
            LoseLife();

            return;
        }

        HandlePaddleCollision();
        HandleBrickCollisions();
    }

    private void HandlePaddleCollision()
    {
        if (!CollisionDetector.BallHitsPaddle(_ball, _paddle))
            return;

        _paddleHitCount++;

        double baseSpeed = GameConstants.BaseBallSpeed + _levelIndex * 25.0;
        int tier = Math.Min(
            _paddleHitCount / GameConstants.PaddleHitsPerSpeedTier, 
            GameConstants.MaxSpeedTiers);

        double tierSpeed = baseSpeed + tier * GameConstants.SpeedTierIncrement;

        double relativeHit = (_ball.X - _paddle.CenterX) / (_paddle.Width / 2.0);
        relativeHit = Math.Clamp(relativeHit, -1.0, 1.0);

        double speed = tierSpeed * (1.0 - GameConstants.EdgeSlowdownFactor * Math.Abs(relativeHit));
        speed = Math.Clamp(speed, baseSpeed, GameConstants.MaxBallSpeed);

        double angle = relativeHit * GameConstants.MaxBounceAngle;

        _ball.VX = speed * Math.Sin(angle);
        _ball.VY = -speed * Math.Cos(angle);

        _ball.Y = _paddle.Y - _ball.Radius;
    }

    private void HandleBrickCollisions()
    {
        Brick? brick = CollisionDetector.FindHitBrick(_ball, _bricks);

        if (brick is null)
            return;

        brick.HitsRemaining--;

        if (brick.HitsRemaining <= 0)
        {
            _renderer.RemoveBrickVisual(brick);
            _bricks.Remove(brick);

            _score += 10;
            _hud.SetScore(_score);
        }
        else
        {
            _renderer.UpdateBrickVisual(brick);
        }

        CollisionDetector.ResolveBrickCollision(_ball, brick.Bounds);

        if (_bricks.Count == 0)
            CompleteLevel();
    }

    private void CompleteLevel()
    {
        if (_levelIndex >= _levels.Length - 1)
        {
            _state = GameState.Won;
            _hud.SetMessage("ALL LEVELS COMPLETE\nPRESS SPACE TO RESTART");
        }
        else
        {
            _state = GameState.LevelComplete;
            _hud.SetMessage("LEVEL COMPLETE\nPRESS SPACE FOR NEXT LEVEL");
        }
    }

    private void LoseLife()
    {
        _lives--;
        _hud.SetLives(_lives);

        if (_lives <= 0)
        {
            _state = GameState.GameOver;
            _hud.SetMessage("GAME OVER\nPRESS SPACE TO RESTART");
        }
        else
        {
            _state = GameState.Ready;
            _hud.SetMessage("PRESS SPACE OR CLICK TO LAUNCH");
        }

        _ball.IsAttached = true;
        _ball.AttachToPaddle(_paddle);
    }

    private void LaunchBall()
    {
        _state = GameState.Playing;

        double angle = (_random.NextDouble() * 2.0 - 1.0) * (Math.PI / 6.0);
        double speed = GameConstants.BaseBallSpeed + _levelIndex * 25.0;

        _ball.Launch(angle, speed);

        _hud.SetMessage(string.Empty);
    }

    private void RestartGame()
    {
        _score = 0;
        _lives = 3;

        LoadLevel(0);
    }

    private void LoadLevel(int index)
    {
        index = Math.Clamp(index, 0, _levels.Length - 1);
        _levelIndex = index;

        _bricks.Clear();
        _renderer.Clear();

        _paddle.X = (GameConstants.FieldWidth - GameConstants.BatWidth) / 2.0;
        _paddleHitCount = 0;

        _bricks.AddRange(
            BrickFactory.CreateBricks(_levels[index], GameConstants.FieldWidth));
        _renderer.RenderBricks(_bricks);
        _renderer.CreatePaddleAndBallVisuals(_paddle, _ball);

        _state = GameState.Ready;
        _ball.IsAttached = true;

        _ball.AttachToPaddle(_paddle);
        _renderer.UpdatePositions(_paddle, _ball);

        _hud.SetScore(_score);
        _hud.SetLevel(_levelIndex + 1);
        _hud.SetLives(_lives);
        _hud.SetMessage("PRESS SPACE OR CLICK TO LAUNCH");
    }

    private readonly string[][] _levels;
    private readonly Random _random = new();
    private readonly List<Brick> _bricks = [];

    private readonly Paddle _paddle;
    private readonly Ball _ball;

    private readonly GameRenderer _renderer;
    private readonly GameHud _hud;

    private GameState _state = GameState.Ready;
    private GameState _stateBeforePause;

    private int _score;
    private int _lives = 3;
    private int _levelIndex;
    private int _paddleHitCount;
}
