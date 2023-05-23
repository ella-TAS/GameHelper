using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Trampoline")]
public class Trampoline : Entity {
    private readonly Sprite sprite;
    private readonly float speedBoostX, speedBoostY;
    private readonly bool facingUpLeft, refillDash, oneUse;
    private bool inside, wasInside;

    public Trampoline(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        speedBoostX = data.Float("speedBoostX");
        speedBoostY = data.Float("speedBoostY");
        facingUpLeft = data.Bool("facingUpLeft");
        refillDash = data.Bool("refillDash");
        oneUse = data.Bool("oneUse");
        sprite = GameHelper.SpriteBank.Create("trampoline");
        if(!facingUpLeft) {
            sprite.FlipX = true;
            sprite.RenderPosition = new Vector2(6f, 0f);
        }
        Add(sprite);
        base.Collider = new Hitbox(16f, 16f);
        base.Depth = -1;
        Add(new PlayerCollider(onCollide));
    }

    private void onCollide(Player player) {
        if(!wasInside) {
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
        if(oneUse) {
            base.Collidable = false;
            sprite.Play("broken");
        }
        inside = true;
    }

    public override void Update() {
        base.Update();
        if(inside && !wasInside) {
            wasInside = true;
            //entered
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            p.StateMachine.State = 0;
            p.AutoJump = true;
            if(!p.Inventory.NoRefills && refillDash) {
                p.RefillDash();
            }
            p.RefillStamina();
        }
        if(!inside && wasInside) {
            wasInside = false;
        }
        inside = false;
    }
}