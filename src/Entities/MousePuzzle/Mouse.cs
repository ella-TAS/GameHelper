using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

[Tracked]
public class Mouse : Actor {
    public enum Direction { Left, Up, Right, Down }
    private Sprite sprite;
    private Direction dir;

    public Mouse(Vector2 Position) : base(Position) {
        base.Collider = new Hitbox(8, 8);
        dir = Direction.Down;
        sprite = GameHelper.SpriteBank.Create("mouse");
        sprite.CenterOrigin();
        sprite.RenderPosition = new Vector2(4, 4);
        Add(sprite);
    }

    public override void Update() {
        base.Update();
        NaiveMove(dirToVector() * 120f * Engine.DeltaTime);
        bool hitRotator = false;
        foreach(MouseRotator m in CollideAll<MouseRotator>()) {
            rotate(m.Clockwise);
            Audio.Play("event:/GameHelper/annoyingmice/rotate");
            hitRotator = true;
        }
        if(CollideCheck<Solid>() && !hitRotator) {
            RemoveSelf();
        }
    }

    private void rotate(bool clockwise) {
        NaiveMove(-dirToVector() * 120f * Engine.DeltaTime);
        dir = (Direction) (((int) dir + (clockwise ? 1 : -1) + 4) % 4);
        sprite.Rotation = (float) (((double) dir + 1) * 0.5 * Math.PI);
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