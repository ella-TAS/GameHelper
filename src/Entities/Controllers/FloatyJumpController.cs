using Monocle;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FloatyJumpController")]
public class FloatyJumpController : Entity {
    internal static bool Floating, CanFloat;
    private readonly bool enable;

#pragma warning disable IDE0060, RCS1163
    public FloatyJumpController(EntityData data, Vector2 levelOffset) {
        enable = data.Bool("enable");
    }
#pragma warning restore

    private static void OnPlayerUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        if(GameHelper.Session.FloatyJumps) {
            if(p.OnGround()) {
                CanFloat = true;
                Floating = false;
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

    public override void Added(Scene scene) {
        base.Added(scene);
        GameHelper.Session.FloatyJumps = enable;
    }

    public static void Hook() {
        On.Celeste.Player.Update += OnPlayerUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.Update -= OnPlayerUpdate;
    }
}