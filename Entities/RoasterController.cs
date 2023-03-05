//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/RoasterController")]
public class RoasterController : Entity {
    private ParticleType pType;
    private Color color;
    private float _timer, timer;
    private int progress;

    public RoasterController(EntityData data, Vector2 levelOffset) {
        _timer = data.Int("timer");
        base.Depth = -9999999;

        //particles
        pType = new ParticleType(){
            Color = Color.Orange,
            Color2 = Color.OrangeRed,
            ColorMode = ParticleType.ColorModes.Choose,
            Direction = 4.7123890f,
            DirectionRange = 0.2f,
            LifeMax = 1,
            LifeMin = 0.2f,
            SpeedMin = 20,
            SpeedMax = 40,
            Size = 1
        };
    }

    public override void Update() {
        base.Update();
        timer++;
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            if(p.OnGround()) {
                SceneAs<Level>().ParticlesFG.Emit(pType, 1, base.BottomLeft, Vector2.UnitX * 8f);
            } else if(!p.CollideCheck<Solid>(p.Position + new Vector2(3, 0)) && !p.CollideCheck<Solid>(p.Position + new Vector2(-3, 0))) {
                timer = 0;
            }
            if(timer >= _timer) {
                p.Die(Vector2.Zero);
                base.Visible = false;
            }
            Position = p.Center + new Vector2(p.Facing == Facings.Right ? -1 : 0, -3);
        }
        float ratio = (float) timer / (float) _timer;
        progress = 15 - (int) (15 * ratio);
        color = new Color(255, (int) (255f * (1 - ratio)), 0);
    }

    public override void Render() {
        if(!SceneAs<Level>().Transitioning) {
            drawCircle(Position, progress, color);
        }
        Draw.Point(Position, Color.Aqua);
    }

    private void drawCircle(Vector2 center, int radius, Color color) {
        int radius2 = radius * radius;
        int y = radius;
        drawCirclePx(0, radius);
        for(int x = 1; x < y; x++) {
            y = (int) Math.Round(Math.Sqrt(radius2 - x*x));
            drawCirclePx(x, y);
        }

        void drawCirclePx(int x, int y) {
            Draw.Point(center + new Vector2(x, y), color);
            Draw.Point(center + new Vector2(-x, y), color);
            Draw.Point(center + new Vector2(x, -y), color);
            Draw.Point(center + new Vector2(-x, -y), color);
            Draw.Point(center + new Vector2(y, x), color);
            Draw.Point(center + new Vector2(-y, x), color);
            Draw.Point(center + new Vector2(y, -x), color);
            Draw.Point(center + new Vector2(-y, -x), color);
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(_timer <= 0) {
            Logger.Log("GameHelper", "RoasterController has bad timer value");
            RemoveSelf();
        }
    }
}