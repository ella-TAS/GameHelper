using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

public class Mouse : Actor {
    public enum Direction {Left, Up, Right, Down}
    private Direction dir;

    public Mouse(Vector2 Position) : base(Position) {
        base.Collider = new Hitbox(8, 8, -4, -4);
        dir = Direction.Down;
    }

    public override void Update() {
        base.Update();
        NaiveMove(dirToVector());
        foreach(MouseRotator m in CollideAll<MouseRotator>()) {
            NaiveMove(-dirToVector());
            rotate(m.Clockwise);
        }
    }

    private void rotate(bool clockwise) {
        dir = (Direction) (((int) dir + (clockwise ? 1 : -1) + 4) % 4);
    }

    private Vector2 dirToVector() {
        switch(dir) {
            case Direction.Left: return -Vector2.UnitX;
            case Direction.Up: return -Vector2.UnitY;
            case Direction.Right: return Vector2.UnitX;
            case Direction.Down: return Vector2.UnitY;
            default: return Vector2.Zero;
        }
    }
}