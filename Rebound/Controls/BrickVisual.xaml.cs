using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rebound.Controls;

internal partial class BrickVisual : UserControl
{
    public BrickVisual(Rect bounds, Color color, int hitsRemaining)
    {
        InitializeComponent();
        SetAppearance(bounds, color, hitsRemaining);
    }

    public void SetAppearance(Rect bounds, Color color, int hitsRemaining)
    {
        Width = bounds.Width + 3.0;
        Height = bounds.Height + 3.0;

        Geometry geometry = CreateGeometry(bounds.Width, bounds.Height, hitsRemaining);

        Shadow.Width = bounds.Width;
        Shadow.Height = bounds.Height;
        Shadow.Data = geometry;

        Canvas.SetLeft(Shadow, 3.0);
        Canvas.SetTop(Shadow, 3.0);

        Main.Width = bounds.Width;
        Main.Height = bounds.Height;
        Main.Data = geometry;

        Main.Fill = new SolidColorBrush(color);

        bool isIndestructible = hitsRemaining > 3;
        Main.Stroke = new SolidColorBrush(isIndestructible
            ? Color.FromRgb(0xF8, 0xF8, 0xF2)
            : Color.FromRgb(0x10, 0x10, 0x18));
        Main.StrokeThickness = isIndestructible ? 3.0 : 1.0;
    }

    private static Geometry CreateGeometry(
        double width,
        double height,
        int hitsRemaining)
        => hitsRemaining switch
        {
            <= 1 => new RectangleGeometry(
                new Rect(0.0, 0.0, width, height), 
                3.0, 
                3.0),

            <= 2 => CreatePolygonGeometry(
            [
                new Point(width / 2.0, 0.0),
                new Point(width, height / 2.0),
                new Point(width / 2.0, height),
                new Point(0.0, height / 2.0)
            ]),

            <= 3 => CreatePolygonGeometry(
                CreateOctagonPoints(width, height)),

            _ => new RectangleGeometry(
                new Rect(0.0, 0.0, width, height), 
                3.0, 
                3.0),
        };

    private static Geometry CreatePolygonGeometry(PointCollection points)
    {
        var figure = new PathFigure 
        { 
            StartPoint = points[0], 
            IsClosed = true, 
            IsFilled = true 
        };

        for (int i = 1; i < points.Count; i++)
        {
            figure.Segments.Add(
                new LineSegment(points[i], isStroked: true));
        }

        return new PathGeometry([figure]);
    }

    private static PointCollection CreateOctagonPoints(double width, double height)
    {
        const double cut = 0.3;
        double cw = width * cut;
        double ch = height * cut;

        return
        [
            new Point(cw, 0.0),
            new Point(width - cw, 0.0),
            new Point(width, ch),
            new Point(width, height - ch),
            new Point(width - cw, height),
            new Point(cw, height),
            new Point(0.0, height - ch),
            new Point(0.0, ch)
        ];
    }
}
