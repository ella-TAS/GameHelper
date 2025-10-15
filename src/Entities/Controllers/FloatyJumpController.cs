using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FloatyJumpController")]
public class FloatyJumpController : Entity {
    private static EventInstance floatySound;
    internal static float actualYSpeed;
    private readonly bool enable;
    private readonly string flag;

    public FloatyJumpController(EntityData data, Vector2 levelOffset) {
        enable = data.Bool("enable");
        flag = data.Attr("flag");
    }

    public override void Update() {
        base.Update();
        if(SceneAs<Level>().Tracker.GetEntity<Player>() == null) {
            Audio.Stop(floatySound);
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(Utils.Util.GetFlag(flag, Scene, true)) {
            GameHelper.Session.FloatyJumps = enable;
        }
    }

    private static void OnPlayerUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        if(GameHelper.Session.FloatyJumps) {
            if(!p.OnGround() && p.Speed.Y > -40f && Input.Jump.Check && p.StateMachine.State == PlayerState.StNormal && p.InControl) {
                if(actualYSpeed.Equals(float.MaxValue)) {
                    actualYSpeed = p.Speed.Y;
                }
                p.Speed.Y = actualYSpeed = Calc.Approach(actualYSpeed, 22.5f, p.Speed.Y > 40 ? 22.5f : 7.5f);
                if(!Audio.IsPlaying(floatySound)) {
                    floatySound = Audio.Play("event:/GameHelper/floatyjump/floating");
                }
            } else {
                actualYSpeed = float.MaxValue;
                Audio.Stop(floatySound);
            }
        }
    }

    private static bool OnTransitionTo(On.Celeste.Player.orig_TransitionTo orig, Player p, Vector2 target, Vector2 direction) {
        Audio.Stop(floatySound);
        return orig(p, target, direction);
    }

    private static void OnDeath(Player self) {
        Audio.Stop(floatySound);
    }

    public static void Hook() {
        On.Celeste.Player.Update += OnPlayerUpdate;
        On.Celeste.Player.TransitionTo += OnTransitionTo;
        Everest.Events.Player.OnDie += OnDeath;
    }

    public static void Unhook() {
        On.Celeste.Player.Update -= OnPlayerUpdate;
        On.Celeste.Player.TransitionTo -= OnTransitionTo;
        Everest.Events.Player.OnDie -= OnDeath;
    }
}