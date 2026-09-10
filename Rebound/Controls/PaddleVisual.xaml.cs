using System.Windows.Controls;

namespace Rebound.Controls;

internal partial class PaddleVisual : UserControl
{
    public PaddleVisual(double width, double height)
    {
        InitializeComponent();

        Width = width + 7.0;
        Height = height + 7.0;

        Shadow.Width = width;
        Shadow.Height = height;

        Canvas.SetLeft(Shadow, 7.0);
        Canvas.SetTop(Shadow, 7.0);

        Bat.Width = width;
        Bat.Height = height;
    }
}
