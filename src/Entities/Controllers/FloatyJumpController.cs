using Monocle;
using Microsoft.Xna.Framework;
using System;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FloatyJumpController")]
public class FloatyJumpController : Entity {
    internal static bool Floating;
    internal static float FloatTimer;
    private bool enable;

    public FloatyJumpController(EntityData data, Vector2 levelOffset) {
        enable = data.Bool("enable");
    }

    public static void OnUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        if(GameHelper.Instance.Session.FloatyJumps) {
            if(p.OnGround()) {
                FloatTimer = 0.5f; //seconds
            } else if(Math.Abs(p.Speed.Y) <= 40f && Input.Jump.Check && p.StateMachine.State == 0 && FloatTimer > 0) {
                FloatTimer -= Engine.DeltaTime;
                p.DummyGravity = false;
                p.Speed.Y = Calc.Approach(p.Speed.Y, 30f, 7.5f);
                Floating = true;
            } else if(Floating) {
                p.DummyGravity = true;
                Floating = false;
            }
        }
    }

    public static void Load() {
        On.Celeste.Player.Update += OnUpdate;
    }

    public static void Unload() {
        On.Celeste.Player.Update -= OnUpdate;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        GameHelper.Instance.Session.FloatyJumps = enable;
    }
}