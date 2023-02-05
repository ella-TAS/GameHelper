//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Bloodhound")]
public class Bloodhound : Entity {
    private int homePos;
    private int moveRange;
    private bool horizontal;

    public Bloodhound(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        moveRange = data.Int("moveRange");
        horizontal = data.Bool("horizontal");
        //base.Collider = new Hitbox(16f, 16f);
        base.Collider = new Circle(6f);
        Hitbox trackRange = new Hitbox(32f, 8f);
        Add(new PlayerCollider(onCollide));
        Add(new PlayerCollider(onSight, trackRange));
    }

    public override void Update() {
        base.Update();
    }

    private void onSight(Player p) {

    }

    private void onCollide(Player p) {
        p.Die(Vector2.Normalize(p.Center - this.Center));
    }
}