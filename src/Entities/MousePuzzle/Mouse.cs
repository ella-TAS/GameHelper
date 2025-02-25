using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

public class Mouse : Actor {
    public enum Direction { Left, Up, Right, Down }
    private readonly Sprite sprite;
    private Direction dir;
    private bool deathRoutine;

    public Mouse(Vector2 Position) : base(Position) {
        Collider = new Hitbox(16, 16);
        Depth = -2;
        dir = Direction.Down;
        sprite = GameHelper.SpriteBank.Create("mouse");
        sprite.CenterOrigin();
        sprite.RenderPosition = new Vector2(8, 8);
        Add(sprite);
    }

    public override void Update() {
        base.Update();
        NaiveMove(dirToVector() * 120f * Engine.DeltaTime);
        if(!deathRoutine && CollideCheck<Solid>()) {
            bool surviveFrame = false;
            foreach(MouseRotator m in CollideAll<MouseRotator>()) {
                rotate(m.Clockwise);
                Audio.Play("event:/GameHelper/annoyingmice/rotate");
                surviveFrame = true;
            }
            foreach(MouseHole n in CollideAll<MouseHole>()) {
                if(n.Complete()) {
                    Add(new Coroutine(routineDestroy()));
                    deathRoutine = true;
                }
                surviveFrame = true;
            }
            if(!surviveFrame) {
                RemoveSelf();
            }
        }
    }

    private IEnumerator routineDestroy() {
        yield return 0.05f;
        RemoveSelf();
    }

    private void rotate(bool clockwise) {
        NaiveMove(-dirToVector() * 120f * Engine.DeltaTime);
        dir = (Direction) (((int) dir + (clockwise ? 1 : -1) + 4) % 4);
        sprite.Rotation = (float) (((double) dir + 1) * 0.5 * Math.PI);
    }

    private Vector2 dirToVector() {
        return dir switch {
            Direction.Left => -Vector2.UnitX,
            Direction.Up => -Vector2.UnitY,
            Direction.Right => Vector2.UnitX,
            Direction.Down => Vector2.UnitY,
            _ => Vector2.Zero,
        };
    }
}