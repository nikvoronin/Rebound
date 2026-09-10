using System.Windows.Controls;

namespace Rebound.Engine;

internal sealed class GameHud(
    TextBlock scoreText,
    TextBlock levelText,
    TextBlock livesText,
    TextBlock messageText)
{
    public void SetScore(int score) => 
        _scoreText.Text = score.ToString("D6");

    public void SetLevel(int levelNumber) => 
        _levelText.Text = levelNumber.ToString();

    public void SetLives(int lives) => 
        _livesText.Text = lives.ToString();

    public void SetMessage(string text) => 
        _messageText.Text = text;

    private readonly TextBlock _scoreText = scoreText;
    private readonly TextBlock _levelText = levelText;
    private readonly TextBlock _livesText = livesText;
    private readonly TextBlock _messageText = messageText;
}
