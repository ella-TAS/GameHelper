//EllaTAS
using System;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Trampoline")]
public class Trampoline : Entity {
    private Sprite sprite;
    private float speedBoostX;
    private float speedBoostY;
    private bool facingUpLeft;
    private bool refillDash;
    private bool oneUse;
    private int hasCollided = 0;
    private int collidable = 0;

    public Trampoline(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        speedBoostX = data.Float("speedBoostX");
        speedBoostY = data.Float("speedBoostY");
        facingUpLeft = data.Bool("facingUpLeft");
        refillDash = data.Bool("refillDash");
        oneUse = data.Bool("oneUse");
        sprite = GameHelperModule.getSpriteBank().Create("trampoline");
        if(!facingUpLeft) {
            sprite.FlipX = true;
            sprite.RenderPosition = new Vector2(6f, 0f);
        }
        Add(sprite);
        base.Collider = new ColliderList(new Hitbox(16f, 16f, 0f, 0f));
        Add(new PlayerCollider(onCollide));
    }

    private void onCollide(Player player) {
        if(collidable == 0) {
            sprite.Play("hit");
            float speedX = player.Speed.X;
            if(facingUpLeft) {
                player.Speed.X = Math.Min(Math.Min(-player.Speed.Y - speedBoostX, -130), player.Speed.X - speedBoostX);
                player.Speed.Y = Math.Min(-speedX - speedBoostY, -200);
            } else {
                player.Speed.X = Math.Max(Math.Max(player.Speed.Y + speedBoostX, 130), player.Speed.X + speedBoostX);
                player.Speed.Y = Math.Min(speedX - speedBoostY, -200);
            }
        }
        collidable++;
        if(oneUse) {
            base.Collidable = false;
            sprite.Play("broken");
        }
    }

    public override void Update() {
        base.Update();
        if(collidable != 0) {
            hasCollided++;
            if(collidable != hasCollided) {
                collidable = hasCollided = 0;
            }
        }
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(collidable == 1){
            p.StateMachine.State = 0;
            p.AutoJump = true;
            if (!p.Inventory.NoRefills && refillDash) {
                p.RefillDash();
            }
        p.RefillStamina();
        }
    }
}