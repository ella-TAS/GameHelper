using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Linq;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Balloon")]
public class Balloon : Entity {
    private static int BalloonCount;
    private readonly Sprite sprite;
    private readonly float floatyOffset;
    private readonly bool oneUse, superBounce;
    private bool isLead, inBubble;

    public Balloon(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        superBounce = data.Bool("superBounce");
        Collider = new Hitbox(15, 8);
        Depth = -1;
        floatyOffset = (int) (-3.15f * GameHelper.Random.NextFloat());
        Add(sprite = GameHelper.SpriteBank.Create("balloon_" + data.Attr("color", "red")));
        Add(new PlayerCollider(onPlayer));
        sprite.Play("idle", true, true);
    }

    public override void Update() {
        base.Update();

        // check if inside a Collidable VortexHelper bubble
        bool collideBubble = false;
        foreach (Type t in SceneAs<Level>().Tracker.Entities.Keys) {
            // check if DashBubbles are tracked
            if (t.ToString() == "Celeste.Mod.VortexHelper.Entities.DashBubble") {
                collideBubble = SceneAs<Level>().Tracker.Entities[t].Any(e => e.Collidable && CollideCheck(e));
                break;
            }
        }
        if (collideBubble && !inBubble) {
            Collidable = false;
            inBubble = true;
        }
        if (!collideBubble && inBubble) {
            Collidable = true;
            inBubble = false;
        }

        sprite.RenderPosition = Position + (1.5f * Vector2.UnitY * (float) Math.Sin(2 * (Engine.Scene.TimeActive + floatyOffset)));
        if (isLead) {
            if (SceneAs<Level>().Tracker.GetEntity<Player>()?.OnGround() == true) {
                BalloonCount = 0;
            }
        }
    }

    private void onPlayer(Player player) {
        if (superBounce) {
            float speedX = player.Speed.X;
            player.SuperBounce(Y);
            player.Speed.X = speedX;
        } else {
            player.Bounce(Y);
        }
        player.AutoJumpTimer = 0f;
        player.Speed.X *= 1.2f;
        sprite.Play("pop");
        Audio.Play("event:/GameHelper/balloon/Balloon_pop", "balloon_count", BalloonCount);
        if (BalloonCount < 7) {
            BalloonCount++;
        }
        Collidable = false;
        Add(new Coroutine(RoutineRespawn()));
    }

    private IEnumerator RoutineRespawn() {
        yield return 2.5f;
        if (oneUse) {
            RemoveSelf();
            yield break;
        }
        Collidable = true;
        sprite.Play("spawn");
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        BalloonCount = -1;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (BalloonCount == -1) {
            isLead = true;
            BalloonCount = 0;
        }
    }
}