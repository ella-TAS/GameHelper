using Monocle;
using System;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/RicochetFist")]
public class RicochetFist : Actor {
    private Sprite sprite;
    private Vector2 direction;
    private int stamina;

    public RicochetFist(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        direction = (data.Nodes[0] - data.Position).SafeNormalize() * data.Float("speed");
        stamina = 4;
        base.Depth = 1;
        base.Collider = new Hitbox(12, 12);
        Add(new PlayerCollider(onCollide));
        sprite = GameHelper.SpriteBank.Create("ricochet_fist");
        sprite.CenterOrigin();
        sprite.Rotation = direction.Angle() - 0.7853982f; // sprite direction 45°
        sprite.RenderPosition = 6f * Vector2.One;
        Add(sprite);
    }

    public override void Update() {
        base.Update();
        bool collideX = MoveH(direction.X * Engine.DeltaTime);
        bool collideY = MoveV(direction.Y * Engine.DeltaTime);
        if(collideX || collideY) {
            Audio.Play("event:/GameHelper/fist/bullet_collide");
            stamina--;
            if(stamina == 0) {
                collideX = collideY = true;
                stamina = 4;
            }
            if(collideX) {
                direction.X = -direction.X;
                sprite.Rotation = 1.5707963f - sprite.Rotation; // 45° sprite rotation
            }
            if(collideY) {
                direction.Y = -direction.Y;
                sprite.Rotation = -1.5707963f - sprite.Rotation;
            }
        }
    }

    private void onCollide(Player p) {
        p.Die(direction.SafeNormalize());
    }
}