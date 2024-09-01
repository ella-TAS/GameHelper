using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FloatyJumpController")]
public class FloatyJumpController : Entity {
    internal static float actualYSpeed;
    private readonly bool enable;
    private readonly string flag;

    public FloatyJumpController(EntityData data, Vector2 levelOffset) {
        enable = data.Bool("enable");
        flag = data.Attr("flag");
    }

    private static void OnPlayerUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        if(GameHelper.Session.FloatyJumps) {
            if(!p.OnGround() && p.Speed.Y > -40f && Input.Jump.Check && p.StateMachine.State == 0 && p.InControl) {
                if(actualYSpeed.Equals(float.MaxValue)) {
                    actualYSpeed = p.Speed.Y;
                }
                p.Speed.Y = actualYSpeed = Calc.Approach(actualYSpeed, 22.5f, p.Speed.Y > 40 ? 22.5f : 7.5f);
            } else {
                actualYSpeed = float.MaxValue;
            }
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(flag == "" || SceneAs<Level>().Session.GetFlag(flag)) {
            GameHelper.Session.FloatyJumps = enable;
        }
    }

    public static void Hook() {
        On.Celeste.Player.Update += OnPlayerUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.Update -= OnPlayerUpdate;
    }
}