using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Chainsaw")]
public class Chainsaw : Entity {
    private const float maxSpeed = 3f;
    private const float accel = 0.2f;
    private readonly Sprite sprite;
    private EventInstance sfx;
    private bool charging, stunned;
    private float speed;
    private readonly Vector2 collidePos1, collidePos2;
    private Vector2 homePos, targetPos;

    public Chainsaw(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        targetPos = data.Nodes[0] + levelOffset;
        homePos = data.Position + levelOffset;
        collidePos1 = homePos + (5.5f * (homePos - targetPos).SafeNormalize());
        collidePos2 = targetPos + (5.5f * (targetPos - homePos).SafeNormalize());
        Depth = -1;
        Collider = new Circle(6f);
        sprite = GameHelper.SpriteBank.Create("chainsaw");
        sprite.Rotation = (targetPos - homePos).Angle();
        sprite.FlipY = data.Bool("flipSprite");
        Add(sprite);
        Add(new PlayerCollider(onCollide));
    }

    public override void Update() {
        base.Update();

        //tracking
        if (
            !charging &&
            !stunned &&
            SceneAs<Level>().Tracker.GetEntity<Player>() is { } player &&
            !player.JustRespawned &&
            Scene.CollideFirst<Player>(collidePos1, collidePos2) != null
        ) {
            charging = true;
            sprite.Play("chasing");
            sfx = Audio.Play("event:/GameHelper/chainsaw/chainsaw_attack");
        }

        //movement
        if (charging) {
            speed = Calc.Approach(speed, maxSpeed, accel);
            Position = Calc.Approach(Position, targetPos, speed);
            if (Position == targetPos) {
                targetPos = homePos;
                homePos = Position;
                charging = false;
                speed = 0;
                sprite.FlipX = !sprite.FlipX;
                sprite.Play("idle");
                Audio.Stop(sfx);
                Add(new Coroutine(routineStunned()));
            }
        }
    }

    private IEnumerator routineStunned() {
        stunned = true;
        yield return 1f / 6f;
        stunned = false;
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