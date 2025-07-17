using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/TimeFreezeController")]
public class TimeFreezeController : Entity {
    public TimeFreezeController() {
        Depth = -1;
    }

    public override void Update() {
        base.Update();
        if(SceneAs<Level>().Tracker.GetEntity<Player>() is Player p) {
            if(
                p.JustRespawned && p.InControl
                && Input.Aim.Value.Length() < 0.3f && !Input.Jump.Pressed && !Input.Dash.Pressed && !Input.CrouchDash.Pressed
            ) {
                if(Engine.TimeRate > 0f) {
                    Engine.TimeRate = Calc.Approach(Engine.TimeRate, 0f, 0.1f);
                    SceneAs<Level>().Session.SetFlag("GameHelper_TimeFrozen", true);
                }
            } else if(Engine.TimeRate < 1f) {
                Engine.TimeRate = 1f;
                p.JustRespawned = false;
                SceneAs<Level>().Session.SetFlag("GameHelper_TimeFrozen", false);
            }
        }
    }
}