using Monocle;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FloatyJumpController")]
public class FloatyJumpController : Entity {
    internal static bool Floating, CanFloat;
    private bool enable;

    public FloatyJumpController(EntityData data, Vector2 levelOffset) {
        enable = data.Bool("enable");
    }

    public static void OnUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        if(GameHelper.Instance.Session.FloatyJumps) {
            if(p.OnGround()) {
                CanFloat = true;
            } else if(CanFloat && Math.Abs(p.Speed.Y) <= 40f && Input.Jump.Check && p.StateMachine.State == 0) {
                p.DummyGravity = false;
                p.Speed.Y = Calc.Approach(p.Speed.Y, 30f, 7.5f);
                Floating = true;
            } else if(Floating) {
                p.DummyGravity = true;
                Floating = CanFloat = false;
            }
        }
    }

    public static void OnCollideH(On.Celeste.Player.orig_OnCollideH orig, Player p, CollisionData data) {
        orig(p, data);
        CanFloat = false;
    }

    public static void Load() {
        On.Celeste.Player.Update += OnUpdate;
        On.Celeste.Player.OnCollideH += OnCollideH;
    }

    public static void Unload() {
        On.Celeste.Player.Update -= OnUpdate;
        On.Celeste.Player.OnCollideH -= OnCollideH;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        GameHelper.Instance.Session.FloatyJumps = enable;
    }
}