using System.IO;
using System.Text.Json;

namespace Rebound.Levels;

// Levels for Krakout-style game, loaded from Levels.json at startup.
// Grid: 15 columns x 11 rows
// '#' = brick
// '.' = empty space
internal static class LevelLoader
{
    public static string[][] LoadLevels()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Levels", "Levels.json");
        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<string[][]>(json)
            ?? throw new InvalidOperationException(
                "Levels.json is empty or invalid.");
    }
}
