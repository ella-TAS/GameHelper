using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Balloon")]
public class Balloon : Entity {
    private static int BalloonCount;
    private Sprite sprite;
    private float floatyOffset;
    private bool isLead;
    private bool oneUse, superBounce;

    public Balloon(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        superBounce = data.Bool("superBounce");
        base.Collider = new Hitbox(15, 8);
        floatyOffset = (int) (-3.15f * GameHelper.Random.NextFloat());
        Add(new PlayerCollider(onCollide));
        Add(sprite = GameHelper.SpriteBank.Create("balloon_" + data.Attr("color", "red")));
        sprite.Play("idle", true, true);
    }

    public override void Update() {
        base.Update();
        sprite.RenderPosition = Position + 1.5f * Vector2.UnitY * (float) Math.Sin(2 * (Engine.Scene.TimeActive + floatyOffset));
        if(isLead) {
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null && p.OnGround()) {
                BalloonCount = 0;
            }
        }
    }

    private IEnumerator routineRespawn() {
        base.Collidable = false;
        yield return 2.5f;
        base.Collidable = true;
        sprite.Play("spawn");
    }

    private void onCollide(Player player) {
        if(superBounce) {
            float speedX = player.Speed.X;
            player.SuperBounce(Position.Y);
            player.Speed.X = speedX;
        } else {
            player.Bounce(Position.Y);
        }
        player.AutoJumpTimer = 0f;
        player.Speed.X *= 1.2f;
        sprite.Play("pop");
        Audio.Play("event:/GameHelper/balloon/Balloon_pop", "balloon_count", BalloonCount);
        if(BalloonCount < 7) {
            BalloonCount++;
        }
        if(!oneUse) {
            Add(new Coroutine(routineRespawn()));
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        BalloonCount = -1;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(BalloonCount == -1) {
            isLead = true;
            BalloonCount = 0;
        }
    }
}