using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/RicochetFist")]
public class RicochetFist : Actor {
    private Vector2 direction;
    private int stamina;

    public RicochetFist(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        direction = (data.Nodes[0] - data.Position).SafeNormalize() * data.Float("speed");
        stamina = 4;
        base.Depth = 1;
        base.Collider = new Hitbox(12, 12);
        Add(new PlayerCollider(onCollide));
    }

    public override void Update() {
        base.Update();
        bool collideX = MoveH(direction.X * Engine.DeltaTime);
        bool collideY = MoveV(direction.Y * Engine.DeltaTime);
        if(collideX || collideY) {
            stamina--;
            if(stamina == 0) {
                direction = -direction;
                stamina = 4;
            } else {
                if(collideX) {
                    direction.X = -direction.X;
                }
                if(collideY) {
                    direction.Y = -direction.Y;
                }
            }
        }
    }

    private void onCollide(Player p) {
        p.Die(direction.SafeNormalize());
    }
}