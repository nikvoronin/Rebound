using System.Windows.Controls;

namespace Rebound.Controls;

internal partial class BallVisual : UserControl
{
    public BallVisual(double radius)
    {
        InitializeComponent();

        double diameter = radius * 2.0;

        Width = diameter + 7.0;
        Height = diameter + 7.0;

        Shadow.Width = diameter;
        Shadow.Height = diameter;

        Canvas.SetLeft(Shadow, 7.0);
        Canvas.SetTop(Shadow, 7.0);

        Ball.Width = diameter;
        Ball.Height = diameter;

        double glareSize = radius * 0.5;
        Glare.Width = glareSize;
        Glare.Height = glareSize;

        Canvas.SetLeft(Glare, radius / 2.0);
        Canvas.SetTop(Glare, radius / 2.0);
    }
}
