using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Bloodhound")]
public class Bloodhound : Actor {
    private const float maxSpeed = 3f;
    private const float accel = 0.2f;
    private const int stunTime = 10;
    private bool charging;
    private int stunned;
    private float speed;
    private Vector2 homePos, targetPos;

    public Bloodhound(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        targetPos = data.Nodes[0] + levelOffset;
        homePos = data.Position + levelOffset;
        charging = false;
        base.Depth = -1;
        base.Collider = new Circle(6f);
        Add(new PlayerCollider(onCollide));
    }

    public override void Update() {
        base.Update();

        //tracking
        if(base.Scene.CollideFirst<Player>(homePos, targetPos) != null && stunned <= 0) {
            charging = true;
        }

        stunned--;

        //movement
        if(charging) {
            speed = Calc.Approach(speed, maxSpeed, accel);
            Position = Calc.Approach(Position, targetPos, speed);
            if(Position == Calc.Approach(Position, targetPos, 1f)) {
                targetPos = homePos;
                homePos = Position;
                stunned = stunTime;
                charging = false;
                speed = 0;
            }
        }
    }


    private void onCollide(Player p) {
        p.Die((p.Position - Position).SafeNormalize() + (charging ? (targetPos - homePos).SafeNormalize() : Vector2.Zero));
    }

    public override void Render() {
        Draw.Circle(Position, 6, Color.Orange, 1);
    }

    public override void DebugRender(Camera camera) {
        base.DebugRender(camera);
        Draw.Line(homePos, targetPos, Color.Aqua);
    }
}