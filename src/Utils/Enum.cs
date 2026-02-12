using Microsoft.Xna.Framework;

namespace Celeste.Mod.GameHelper.Utils;

public enum Direction {
    Left,
    Up,
    Right,
    Down
}

public static class DirectionExtension {
    public static Vector2 ToVector(this Direction dir) {
        return dir switch {
            Direction.Left => -Vector2.UnitX,
            Direction.Up => -Vector2.UnitY,
            Direction.Right => Vector2.UnitX,
            Direction.Down => Vector2.UnitY,
            _ => Vector2.Zero,
        };
    }

    public static Direction RotateClock(this Direction dir) {
        return (Direction) (((int) dir + 1) % 4);
    }

    public static Direction RotateCounter(this Direction dir) {
        return (Direction) (((int) dir + 3) % 4);
    }

    public static Direction Mirror(this Direction dir) {
        return (Direction) (((int) dir + 2) % 4);
    }

    public static bool isHorizontal(this Direction dir) {
        return dir == Direction.Left || dir == Direction.Right;
    }
}