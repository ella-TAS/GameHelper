//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/RoasterController")]
public class RoasterController : Entity {
    private float _timer, timer, progress;

    public RoasterController(EntityData data, Vector2 levelOffset) {
        _timer = data.Int("timer");
        base.Depth = -9999999;
    }

    public override void Update() {
        base.Update();
        timer++;
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            if(!p.OnGround() && !p.CollideCheck<Solid>(p.Position + new Vector2(3, 0)) && !p.CollideCheck<Solid>(p.Position + new Vector2(-3, 0))) {
                timer = 0;
            }
            if(timer >= _timer) {
                p.Die(Vector2.Zero);
            }
            Position = p.Center + new Vector2(0, -3);
        }
        progress = (int) (14 * timer / _timer + 1);
    }

    public override void Render() {
        Draw.Circle(Position, 15 - progress, progress < 7 ? Color.Aqua : Color.Red, progress, 10);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(_timer <= 0) {
            Logger.Log("GameHelper", "RoasterController has bad timer value");
            RemoveSelf();
        }
    }
}