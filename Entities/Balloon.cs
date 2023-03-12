using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Balloon")]
public class Balloon : Entity {
    private Sprite sprite;
    private bool isController;
    private int respawnTimer, timeActive;
    private bool oneUse;

    public Balloon(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        base.Collider = new Hitbox(15, 8);
        respawnTimer = (int) (-189 * GameHelperModule.Random.NextFloat());
        Add(new PlayerCollider(onCollide));
        Add(sprite = GameHelperModule.GetSpriteBank().Create("balloon_" + data.Attr("color", "red")));
    }

    public override void Update() {
        base.Update();
        respawnTimer--;
        if(respawnTimer == 0 && !oneUse) {
            base.Collidable = true;
            sprite.Play("spawn");
        }
        sprite.RenderPosition = Position + 1.5f * Vector2.UnitY * (float) Math.Sin((float) respawnTimer / 30);

        if(isController) {
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p == null || p.OnGround()) {
                GameHelperModule.BalloonCount = 0;
            }
        }
    }

    private void onCollide(Player player) {
        player.Bounce(Position.Y);
        player.Speed.X *= 1.2f;
        base.Collidable = false;
        respawnTimer = 150;
        sprite.Play("pop");
        Audio.Play("event:/GameHelper/balloon/Balloon_pop", "balloon_count", GameHelperModule.BalloonCount);
        GameHelperModule.IncreaseBalloon();
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        GameHelperModule.BalloonCount = 0;
        isController = SceneAs<Level>().Entities.AmountOf<Balloon>() == 1;
    }
}