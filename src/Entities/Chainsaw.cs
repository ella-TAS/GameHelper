using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using FMOD.Studio;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Chainsaw")]
public class Chainsaw : Entity {
    private const float maxSpeed = 3f;
    private const float accel = 0.2f;
    private Sprite sprite;
    private EventInstance sfx;
    private bool charging;
    private float stunned, speed;
    private Vector2 homePos, targetPos, collidePos1, collidePos2;

    public Chainsaw(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        targetPos = data.Nodes[0] + levelOffset;
        homePos = data.Position + levelOffset;
        collidePos1 = homePos + 5.5f * (homePos - targetPos).SafeNormalize();
        collidePos2 = targetPos + 5.5f * (targetPos - homePos).SafeNormalize();
        base.Depth = -1;
        base.Collider = new Circle(6f);
        sprite = GameHelper.SpriteBank.Create("chainsaw");
        sprite.Rotation = (targetPos - homePos).Angle();
        sprite.FlipY = data.Bool("flipSprite");
        Add(sprite);
        Add(new PlayerCollider(onCollide));
    }

    public override void Update() {
        base.Update();

        //tracking
        if(!charging && stunned <= 0 && base.Scene.CollideFirst<Player>(collidePos1, collidePos2) != null) {
            charging = true;
            sprite.Play("chasing");
            sfx = Audio.Play("event:/GameHelper/chainsaw/chainsaw_attack");
        }

        stunned -= Engine.DeltaTime;

        //movement
        if(charging) {
            speed = Calc.Approach(speed, maxSpeed, accel);
            Position = Calc.Approach(Position, targetPos, speed);
            if(Position == Calc.Approach(Position, targetPos, 1f)) {
                targetPos = homePos;
                homePos = Position;
                stunned = 1f / 6f;
                charging = false;
                speed = 0;
                sprite.FlipX = !sprite.FlipX;
                sprite.Play("idle");
                Audio.Stop(sfx);
            }
        }
    }


    private void onCollide(Player p) {
        p.Die((p.Position - Position).SafeNormalize() + (charging ? (targetPos - homePos).SafeNormalize() : Vector2.Zero));
    }

    public override void DebugRender(Camera camera) {
        base.DebugRender(camera);
        Draw.Line(collidePos1, collidePos2, Color.Aqua);
    }

    public override void Removed(Scene scene) {
        Audio.Stop(sfx);
        base.Removed(scene);
    }
}