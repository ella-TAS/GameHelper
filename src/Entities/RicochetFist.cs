using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/RicochetFist")]
public class RicochetFist : Actor {
    private readonly Image sprite;
    private Vector2 homePos, direction;
    private int stamina;

    public RicochetFist(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        homePos = Position;
        direction = (data.Nodes[0] - data.Position).SafeNormalize() * data.Float("speed");
        stamina = 8;
        base.Depth = 1;
        base.Collider = new Hitbox(12, 12);
        Add(new PlayerCollider(onCollide));
        sprite = new(GFX.Game[data.Attr("sprite", "objects/GameHelper/ricochet_fist")]) {
            Rotation = direction.Angle() - 0.7853982f, // sprite direction 45°
            RenderPosition = 6f * Vector2.One
        };
        sprite.CenterOrigin();
        Add(sprite);
    }

    public override void Update() {
        base.Update();
        bool collideX = MoveH(direction.X * Engine.DeltaTime);
        bool collideY = MoveV(direction.Y * Engine.DeltaTime);
        if(collideX || collideY) {
            Audio.Play("event:/GameHelper/fist/bullet_collide");
            stamina--;
            if(stamina == 4) {
                collideX = collideY = true;
            } else if(stamina == 0) {
                collideX = collideY = true;
                Position = homePos;
                stamina = 8;
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