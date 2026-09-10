# Rebound

A desktop Breakout/Arkanoid-style brick-breaker built with [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/) and .NET.

![rebound-inplay_the-first-level](https://github.com/user-attachments/assets/3f16d787-2784-4d32-8b2c-28b68eec12dd)

Brick-breaker games trace back to Atari's *Breakout* (1976), created by
Nolan Bushnell and Steve Bristow and famously prototyped by Steve Wozniak
and Steve Jobs. The core loop — bounce a ball off a paddle to smash a wall
of bricks without letting it fall — went on to inspire countless
variations, most notably Taito's *Arkanoid* (1986), which added colorful
brick tiers, per-level layouts, and paddle-steered bounce angles. Rebound
is a small, modern take on that same formula: single-screen levels, a
tunable ball-speed curve, and a paddle-position-based bounce mechanic in
the spirit of *Arkanoid*.

## How to play

- **Move the paddle**: `Left`/`Right` arrow keys, or move the mouse across
  the play field.
- **Launch the ball / advance / restart**: `Space` or left mouse click.
  - While a level is ready (ball resting on the paddle), it launches the
    ball.
  - When a level is complete, it advances to the next level.
  - After Game Over or clearing all levels, it restarts from level 1.
- **Quit**: `Escape`.
- **Lives**: you start with 3. Losing the ball (letting it fall past the
  bottom of the field) costs one life; the ball then re-attaches to the
  paddle. Reaching 0 lives ends the game.
- **Scoring**: destroying a brick awards 10 points.
- **Winning a level**: clear every brick to advance. Clearing the final
  level shows "All levels complete."

## Levels

Levels are defined in `Levels.json` as an array of levels, each a `15
column × 11 row` grid of strings:

- `.` — empty space
- `#` — a brick that breaks in a single hit
- `1`–`4` — a brick that takes that many hits to break

Each row picks its brick color from a fixed 6-color palette (indexed by
row), so a level's layout also controls its color banding. The grid is
turned into brick data by `BrickFactory.CreateBricks`, which computes brick
width/height from the field size and grid dimensions.

Clearing all bricks in a level automatically advances to the next one; the
ball's base speed increases slightly with each level index, so later levels
play a bit faster from the start.

## Physics

The simulation runs on a fixed logical play field (600×700 units),
independent of window/DPI scaling, and is integrated every frame using
delta-time (`GameEngine.Update`, `Ball.MoveAndBounceOffWalls`):

- **Wall bounces**: the ball reflects off the left, right, and top edges;
  falling past the bottom triggers a lost life.
- **Paddle steering**: where the ball hits the paddle (relative to its
  center) determines the bounce angle, up to a maximum of 60°, similar to
  classic Arkanoid-style paddle control.
- **Speed tiers**: consecutive paddle hits raise the ball's speed in steps
  up to a capped maximum, and hits near the paddle's edges shave a bit of
  speed off (`GameEngine.HandlePaddleCollision`), keeping center hits the
  most "powerful."
- **Brick collision**: a circle-vs-rectangle test
  (`CollisionDetector.CircleIntersectsRect`) detects hits, and
  `CollisionDetector.ResolveBrickCollision` pushes the ball back out of the
  brick and reflects its velocity along whichever axis was penetrated more
  deeply, so bounces off corners and edges look correct.

## Graphics

The **paddle**, **ball**, and each **brick** are each a single composite
visual — a `PaddleVisual`, `BallVisual`, or `BrickVisual` `UserControl` —
that bundles a blurred, offset drop-shadow together with the solid-color
main shape (and, for the ball, a small yellow ellipse "glare" highlight)
into one object. `GameRenderer` only ever creates, positions, and removes
one of these composites per entity, rather than tracking several
independent shapes:

- The **paddle** and **ball** composites are a `Rectangle`/`Ellipse` pair
  (shadow + main shape), with the ball adding its glare ellipse on top.
- **Bricks** change shape with their hit-tier — rounded rectangle (1 hit),
  diamond (2 hits), octagon (3 hits), and a thick-bordered rectangle (4+
  hits) — rendered as a `Path` whose `Geometry` is rebuilt in place by
  `BrickVisual.SetAppearance` whenever a brick is hit, so its shadow
  always matches its current shape.
- Every shape uses `BitmapCache` to keep redraws cheap every frame.

## Project structure

The game logic is split by responsibility:

- `GameEngine` — state machine and per-frame orchestration (the only class
  that ties everything else together).
- `Ball`, `Paddle`, `Brick` — plain physics/data models, no WPF dependency.
- `CollisionDetector` — circle/rect collision tests and response.
- `BrickFactory` — turns a level's character grid into `Brick` data.
- `LevelLoader` — loads and deserializes `Levels.json`.
- `GameConstants`, `GameState` — tuning constants and the state-machine enum.
- `GameRenderer` — creates, positions, and removes the composite visuals
  below on the game `Canvas`.
- `PaddleVisual`, `BallVisual`, `BrickVisual` — XAML `UserControl`s that
  each bundle a shadow shape (and, for the ball, a glare highlight) with
  the main shape into a single positioned composite.
- `GameHud` — updates the score/level/lives/message text.
- `MainWindow.xaml`/`MainWindow.xaml.cs` — the window shell: wires input
  events and drives the per-frame render loop.

## Running the game

```shell
dotnet build
dotnet run
```

Targets `net10.0-windows` as a Windows desktop (`WinExe`) application.

## History

- Combined each entity's
  shadow (and the ball's glare) with its main shape into a single
  `PaddleVisual`/`BallVisual`/`BrickVisual` control, positioned and
  updated as one object instead of several independently-tracked shapes.
- Removed the neon `DropShadowEffect` glow from
  the paddle, ball, bricks, HUD text, and field border, replacing it with
  a plain blurred drop-shadow (now including bricks, which previously had
  none).
- Moved level layouts out of code
  and into `Levels.json`, loaded at startup.
- Added speed tiers that ramp up with consecutive
  paddle hits, capped at a maximum, with edge hits slightly slower than
  center hits.
- Fixed playfield/brick boundary correction so
  collisions resolve against the right edges.
- Added multi-hit bricks (2–4 hits) with distinct shapes per tier.
- Added blurred drop-shadow shapes under the paddle and
  ball for a layered look.
- Added a small highlight ellipse on the ball.
- Initial project scaffolding.
