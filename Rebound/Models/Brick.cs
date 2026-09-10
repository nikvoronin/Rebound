using System.Windows;
using System.Windows.Media;

namespace Rebound.Models;

internal sealed class Brick(
    Rect bounds, 
    int hitsRemaining, 
    Color color)
{
    public Rect Bounds { get; } = bounds;
    public int HitsRemaining { get; set; } = hitsRemaining;
    public Color Color { get; } = color;
}
