using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/DashMagnet")]
[Tracked]
public class DashMagnet : Entity {
    private static bool InsideMagnet;
    private static Vector2 Direction;
    private static float Speed;
    private static Vector2[] RenderPoints;
    private readonly Sprite sprite;
    private bool inside, wasInside, used;
    private readonly bool bulletTime;

    public DashMagnet(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        base.Collider = new Circle(30, 8, 8);
        bulletTime = data.Bool("bulletTime");
        Add(sprite = GameHelper.SpriteBank.Create("dash_magnet"));
        Add(new PlayerCollider(onCollide));
        Depth = 1;
    }

    private void onCollide(Player p) {
        if(p.StateMachine.State == 2) {
            if(!used) {
                sprite.Play("flash");
                Direction = (Center - p.Center).SafeNormalize();
                if(Direction == Vector2.Zero) {
                    Direction = -Vector2.UnitY;
                }
                used = true;
            }
            p.DashDir = Direction;
            p.Speed = Speed * Direction;
            Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, 0.1f);
        } else if(bulletTime && p.Dashes > 0) {
            Engine.TimeRate = Calc.Approach(Engine.TimeRate, 0.2f, 0.1f);
        }
        inside = true;
    }

    public override void Update() {
        base.Update();
        if(inside && !wasInside) {
            wasInside = true;
            InsideMagnet = true;
        }
        if(!inside && wasInside) {
            wasInside = used = false;
            InsideMagnet = false;
            Engine.TimeRate = 1f;
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null && p.StateMachine.State == 2 && !p.StartedDashing) {
                p.StateMachine.State = 0;
            }
        }
        inside = false;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        InsideMagnet = false;
    }

    public override void SceneEnd(Scene scene) {
        base.SceneEnd(scene);
        InsideMagnet = false;
    }

    private static void DashBegin(On.Celeste.Player.orig_DashBegin orig, Player p) {
        const float DiagDashSpeed = 169.70562748f;
        Vector2 s;
        s.Y = Calc.Max(Math.Abs(p.Speed.Y), DiagDashSpeed);
        s.X = Calc.Max(Math.Abs(p.Speed.X), DiagDashSpeed);
        Speed = Calc.Max(s.Length() * 1.2f, 300f);
        orig(p);
    }

    private static IEnumerator DashCoroutine(On.Celeste.Player.orig_DashCoroutine orig, Player p) {
        const float sqrt2 = 1.41421353816986083984f;
        IEnumerator origEnum = orig(p);

        while(origEnum.MoveNext()) {
            yield return origEnum.Current;
            if(InsideMagnet) {
                while(true) {
                    if(p.OnGround()) {
                        p.Speed = Vector2.UnitX * p.Speed.Length() / sqrt2 * (p.Speed.X > 0 ? 1 : -1);
                    }
                    yield return null;
                }
            }
        }
    }

    public static void Load() {
        //precalc indicator offsets
        RenderPoints = new Vector2[36];
        for(int i = 0; i < 36; i++) {
            RenderPoints[i] = Calc.AngleToVector(0.1745329f * i, 30f); //10°
        }

        On.Celeste.Player.DashCoroutine += DashCoroutine;
        On.Celeste.Player.DashBegin += DashBegin;
    }

    public static void Unload() {
        On.Celeste.Player.DashCoroutine -= DashCoroutine;
        On.Celeste.Player.DashBegin -= DashBegin;
    }

    public override void Render() {
        base.Render();
        foreach(Vector2 pos in RenderPoints) {
            Draw.Point(Center + pos, Color.White);
        }
    }
}