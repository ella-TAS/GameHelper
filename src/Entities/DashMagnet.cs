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
    private Sprite sprite;
    private bool inside, wasInside;

    public DashMagnet(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        base.Collider = new Circle(30, 8, 8);
        Add(sprite = GameHelper.SpriteBank.Create("dash_magnet"));
        Add(new PlayerCollider(onCollide));
    }

    private void onCollide(Player p) {
        if(p.StateMachine.State == 2) {
            if(!wasInside && !InsideMagnet) {
                //entered
                sprite.Play("flash");
                Direction = (Center - p.Center).SafeNormalize();
                if(Direction == Vector2.Zero) {
                    Direction = -Vector2.UnitY;
                }
            }
            p.DashDir = Direction;
            p.Speed = Speed * Direction;
            inside = true;
        }
    }

    public override void Update() {
        base.Update();
        if(inside && !wasInside) {
            wasInside = true;
            InsideMagnet = true;
        }
        if(!inside && wasInside) {
            wasInside = false;
            InsideMagnet = false;
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null) {
                p.StateMachine.ForceState(0);
            }
        }
        inside = false;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        InsideMagnet = false;
    }

    private static void DashBegin(On.Celeste.Player.orig_DashBegin orig, Player p) {
        const float DiagDashSpeed = 169.70562748f;
        Vector2 s = p.Speed;
        s.Y = DiagDashSpeed;
        s.X = Calc.Max(Math.Abs(s.X), DiagDashSpeed);
        Speed = s.Length();
        orig(p);
    }

    private static IEnumerator DashCoroutine(On.Celeste.Player.orig_DashCoroutine orig, Player p) {
        IEnumerator origEnum = orig(p);
        while(true) {
            if(InsideMagnet) {
                //magnet dash, cancelled by magnet
                yield return float.MaxValue;
            }
            if(!origEnum.MoveNext()) {
                break;
            }
            yield return origEnum.Current;
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