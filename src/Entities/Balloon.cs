using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Balloon")]
public class Balloon : Entity {
    private Sprite sprite;
    private float respawnTimer;
    private bool isLead;
    private bool oneUse, superBounce;

    public Balloon(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        superBounce = data.Bool("superBounce");
        base.Collider = new Hitbox(15, 8);
        respawnTimer = (int) (-3.15f * GameHelper.Random.NextFloat());
        Add(new PlayerCollider(onCollide));
        Add(sprite = GameHelper.SpriteBank.Create("balloon_" + data.Attr("color", "red")));
        sprite.Play("idle", true, true);
    }

    public override void Update() {
        base.Update();
        respawnTimer -= Engine.DeltaTime;
        if(respawnTimer <= 0 && !oneUse && !base.Collidable) {
            base.Collidable = true;
            sprite.Play("spawn");
        }
        sprite.RenderPosition = Position + 1.5f * Vector2.UnitY * (float) Math.Sin(2 * respawnTimer);
        if(isLead) {
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p == null || p.OnGround()) {
                GameHelper.BalloonCount = 0;
            }
        }
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
        base.Collidable = false;
        respawnTimer = 2.5f;
        sprite.Play("pop");
        Audio.Play("event:/GameHelper/balloon/Balloon_pop", "balloon_count", GameHelper.BalloonCount);
        if(GameHelper.BalloonCount < 7) {
            GameHelper.BalloonCount++;
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        GameHelper.BalloonCount = -1;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(GameHelper.BalloonCount == -1) {
            isLead = true;
            GameHelper.BalloonCount = 0;
        }
    }
}