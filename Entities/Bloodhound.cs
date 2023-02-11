//EllaTAS
using System;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Bloodhound")]
public class Bloodhound : Actor {
    private Vector2 homePos, targetPos;
    private bool facingRightOrUp, charging;
    private bool horizontal;

    public Bloodhound(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        targetPos = data.Nodes[0];
        horizontal = data.Bool("horizontal");
        homePos = data.Position;
        //base.Collider = new Hitbox(16f, 16f);
        base.Collider = new Circle(6f);
        Hitbox trackRange = new Hitbox(32f, 8f);

        Entity e = new Entity();
        e.Collider = trackRange;

        Add(new PlayerCollider(onCollide));
        Add(new PlayerCollider(onSight, trackRange));
    }

    public override void Update() {
        base.Update();

        //movement
        if(charging) {
            base.Collider.Position += Vector2.Normalize(targetPos - Position);
            if(Vector2.Distance(targetPos, base.Collider.Position) < 2) {
                charging = false;
                targetPos = homePos;
                homePos = Position;
            }
        }
    }

    private void onSight(Player p) {
        charging = true;
    }

    private void onCollide(Player p) {
        p.Die(Vector2.Normalize(p.Center - this.Center));
    }
}