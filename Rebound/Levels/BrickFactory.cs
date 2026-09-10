using System.Windows;
using System.Windows.Media;

using Rebound.Models;

namespace Rebound.Levels;

internal static class BrickFactory
{
    private static readonly Color[] Palette =
    {
        Color.FromRgb(0xFF, 0x55, 0x55),
        Color.FromRgb(0xFF, 0xB8, 0x6C),
        Color.FromRgb(0xF1, 0xFA, 0x8C),
        Color.FromRgb(0x50, 0xFA, 0x7B),
        Color.FromRgb(0x8B, 0xE9, 0xFD),
        Color.FromRgb(0xBD, 0x93, 0xF9)
    };

    public static List<Brick> CreateBricks(string[] level, double fieldWidth)
    {
        var bricks = new List<Brick>();

        int columns = level.Max(line => line.Length);

        double brickWidth = fieldWidth / columns;
        double brickHeight = 24.0;
        double top = 64.0;
        double verticalGap = 6.0;

        for (int row = 0; row < level.Length; row++)
        {
            string line = level[row];

            for (int column = 0; column < line.Length; column++)
            {
                char symbol = line[column];

                int hitsRemaining = symbol switch
                {
                    '#' => 1,
                    >= '1' and <= '4' => symbol - '0',
                    _ => 0
                };

                if (hitsRemaining <= 0)
                    continue;

                double x = column * brickWidth + 2.0;
                double y = top + row * (brickHeight + verticalGap);
                double width = brickWidth - 4.0;
                Rect bounds = new(x, y, width, brickHeight);

                bricks.Add(new Brick(
                    bounds, 
                    hitsRemaining, 
                    Palette[row % Palette.Length]));
            }
        }

        return bricks;
    }
}
