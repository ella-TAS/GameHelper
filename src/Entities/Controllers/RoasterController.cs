using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[Tracked]
[CustomEntity("GameHelper/RoasterController")]
public class RoasterController : Entity {
    private readonly ParticleType pType;
    private Color color;
    private readonly string flag;
    private readonly float maxTimer;
    private float timer;
    private readonly bool waterOnly;
    private float progress;

    public RoasterController(EntityData data, Vector2 levelOffset) {
        timer = maxTimer = data.Float("timer");
        flag = data.Attr("flag");
        waterOnly = data.Bool("OnlyExtinguishInWater");
        base.Depth = -9999999;

        //particles
        pType = new ParticleType() {
            Color = Color.Orange,
            Color2 = Color.OrangeRed,
            ColorMode = ParticleType.ColorModes.Choose,
            DirectionRange = 0.5f,
            LifeMax = 1,
            LifeMin = 0.2f,
            SpeedMin = 5,
            SpeedMax = 15,
            Acceleration = Vector2.UnitY * 0.1f,
            Friction = 0.9f,
            Size = 1
        };
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            SceneAs<Level>().Session.SetFlag(flag, true);
            bool ground = p.OnGround();
            bool wallL = p.CollideCheck<Solid>(p.Position + new Vector2(-3, 0));
            bool wallR = p.CollideCheck<Solid>(p.Position + new Vector2(3, 0));
            if((!waterOnly && !ground && !wallL && !wallR) ||
                (waterOnly && p.CollideCheck<Water>()) ||
                (!p.InControl && p.JustRespawned)) {
                ResetTimer();
            }
            if(maxTimer != timer) {
                createParticles(waterOnly, ground, wallL, wallR, p.Facing == Facings.Right);
            }
            if(timer <= 0) {
                p.Die(Vector2.Zero);
                base.Visible = false;
            }
            Position = p.Center + new Vector2(p.Facing == Facings.Right ? -1 : 0, -3);
        }
        float ratio = timer / maxTimer;
        progress = 15 - (15 * (1 - ratio));
        color = new Color(255, (int) (255f * ratio), 0);
        timer -= Engine.DeltaTime;
    }

    private void createParticles(bool water, bool ground = false, bool wallL = false, bool wallR = false, bool facing_right = false) {
        if(water) {
            SceneAs<Level>().ParticlesFG.Emit(pType, 1, base.Position + new Vector2(facing_right ? 2 : 1, 5), Vector2.One * 6f, 4.7123890f);
        } else {
            if(ground) {
                SceneAs<Level>().ParticlesFG.Emit(pType, 1, base.Position + new Vector2(facing_right ? 2 : 1, 10), Vector2.UnitX * 4f, 4.7123890f);
            }
            if(wallR) {
                SceneAs<Level>().ParticlesFG.Emit(pType, 1, base.Position + new Vector2(6, 3), new Vector2(0, 6), 3.9269908f);
            }
            if(wallL) {
                SceneAs<Level>().ParticlesFG.Emit(pType, 1, base.Position + new Vector2(-4, 3), new Vector2(0, 6), 5.4977871f);
            }
        }
    }

    public void ResetTimer() {
        timer = maxTimer;
        SceneAs<Level>().Session.SetFlag(flag, false);
    }

    public override void Render() {
        if(!SceneAs<Level>().Transitioning) {
            drawCircle(Position, progress, color);
        }
    }

    private static void drawCircle(Vector2 center, float radius, Color color) {
        float radius2 = (radius * radius) - 0.25f;
        int y = (int) Math.Round(radius);
        drawCirclePx(0, y);
        for(int x = 1; x < y; x++) {
            //(y-0.5)² = y²-y+0.25
            if((x * x) + (y * y) - y > radius2) {
                y--;
            }
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
        if(maxTimer <= 0) {
            Logger.Warn("GameHelper", "RoasterController has bad timer value in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
    }

    public override void Removed(Scene scene) {
        SceneAs<Level>().Session.SetFlag(flag, false);
        base.Removed(scene);
    }
}