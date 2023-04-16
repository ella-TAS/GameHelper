using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

[Tracked]
[CustomEntity("GameHelper/MouseRotator")]
public class MouseRotator : Solid {
    public Vector2 Movement;
    public bool Clockwise;

    public MouseRotator(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, 16, 16, safe: false) {
        Clockwise = data.Bool("clockwise");
        base.OnDashCollide = OnDashed;
        base.Depth = -1;
        Sprite sprite = GameHelper.SpriteBank.Create("mouse_rotator");
        if(Clockwise) {
            sprite.Play("left");
        }
        Add(sprite);
    }

    public override void Update() {
        base.Update();
        if(Movement != Vector2.Zero) {
            bool collideX = MoveHCollideSolidsAndBounds(SceneAs<Level>(), Movement.X * 120 * Engine.DeltaTime, false);
            bool collideY = MoveVCollideSolidsAndBounds(SceneAs<Level>(), Movement.Y * 120 * Engine.DeltaTime, false, null);
            if(collideX || collideY) {
                Movement = Vector2.Zero;
                Audio.Play("event:/GameHelper/annoyingmice/stop");
            }
        }
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction) {
        Movement = direction;
        if(CollideCheck<Solid>(Position + Movement)) {
            bool chain = false;
            foreach(MouseRotator m in CollideAll<MouseRotator>(Position + Movement)) {
                m.Movement = Movement;
                Movement = Vector2.Zero;
                chain = true;
            }
            if(!chain) {
                Movement = -direction;
            }
        }
        Audio.Play("event:/GameHelper/annoyingmice/hit");
        return DashCollisionResults.Rebound;
    }
}