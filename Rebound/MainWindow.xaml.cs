using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Rebound.Engine;

namespace Rebound;

public partial class MainWindow : Window
{
    private readonly GameEngine _engine;

    private double _lastSeconds = -1.0;

    public MainWindow()
    {
        InitializeComponent();

        var renderer = new GameRenderer(GameCanvas);
        var hud = new GameHud(ScoreText, LevelText, LivesText, MessageText);
        _engine = new GameEngine(renderer, hud);

        CompositionTarget.Rendering += OnFrame;
        Unloaded += (_, _) => CompositionTarget.Rendering -= OnFrame;
    }

    private void OnFrame(object? sender, EventArgs e)
    {
        double seconds = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;

        if (_lastSeconds < 0.0)
        {
            _lastSeconds = seconds;
            return;
        }

        double dt = seconds - _lastSeconds;
        _lastSeconds = seconds;

        if (dt <= 0.0)
            return;

        bool leftPressed = Keyboard.IsKeyDown(Key.Left);
        bool rightPressed = Keyboard.IsKeyDown(Key.Right);

        _engine.Update(dt, leftPressed, rightPressed);
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (!e.IsRepeat)
                _engine.TogglePause();

            e.Handled = true;
            return;
        }

        if (e.Key != Key.Space 
            || e.IsRepeat)
        {
            return;
        }

        _engine.PrimaryAction();

        e.Handled = true;
    }

    private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _engine.PrimaryAction();
    }

    private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        Point position = e.GetPosition(GameCanvas);
        _engine.SetBatPositionFromPointer(position.X);
    }
}
