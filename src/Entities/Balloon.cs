using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Balloon")]
public class Balloon : Entity {
    private static int BalloonCount;
    private readonly Sprite sprite;
    private readonly float floatyOffset;
    private readonly bool oneUse, superBounce;
    private bool isLead;

    public Balloon(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        superBounce = data.Bool("superBounce");
        base.Collider = new Hitbox(15, 8);
        base.Depth = -1;
        floatyOffset = (int) (-3.15f * GameHelper.Random.NextFloat());
        Add(sprite = GameHelper.SpriteBank.Create("balloon_" + data.Attr("color", "red")));
        sprite.Play("idle", true, true);
    }

    public override void Update() {
        base.Update();
        if(Collidable && CollideCheck<Player>()) {
            //moved to Update() to avoid OoO messup with dash bubbles
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
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
            base.Collidable = false;
            Add(new Coroutine(RoutineRespawn()));
        }
        sprite.RenderPosition = Position + (1.5f * Vector2.UnitY * (float) Math.Sin(2 * (Engine.Scene.TimeActive + floatyOffset)));
        if(isLead) {
            if(SceneAs<Level>().Tracker.GetEntity<Player>()?.OnGround() == true) {
                BalloonCount = 0;
            }
        }
    }

    private IEnumerator RoutineRespawn() {
        yield return 2.5f;
        if(oneUse) {
            RemoveSelf();
            yield break;
        }
        base.Collidable = true;
        sprite.Play("spawn");
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