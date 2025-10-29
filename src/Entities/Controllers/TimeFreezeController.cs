using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/TimeFreezeController")]
public class TimeFreezeController : Entity {
    private TimeRateModifier timeRateModifier;

    public TimeFreezeController() {
        Depth = -1;
        Add(timeRateModifier = new TimeRateModifier(1f));
    }

    public override void Update() {
        base.Update();
        if(SceneAs<Level>().Tracker.GetEntity<Player>() is Player p) {
            if(
                p.JustRespawned && p.InControl
                && Input.Aim.Value.Length() < 0.3f && !Input.Jump.Pressed && !Input.Dash.Pressed && !Input.CrouchDash.Pressed
            ) {
                if(timeRateModifier.Multiplier > 0f) {
                    timeRateModifier.Multiplier = Calc.Approach(timeRateModifier.Multiplier, 0f, 0.1f);
                    SceneAs<Level>().Session.SetFlag("GameHelper_TimeFrozen", true);
                }
            } else if(timeRateModifier.Multiplier < 1f) {
                timeRateModifier.Multiplier = 1f;
                p.JustRespawned = false;
                SceneAs<Level>().Session.SetFlag("GameHelper_TimeFrozen", false);
            }
        }
    }
}