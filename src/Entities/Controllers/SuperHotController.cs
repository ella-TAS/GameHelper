using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/SuperHotController")]
public class SuperHotController : Entity {
    public SuperHotController() {
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p == null) {
            Engine.TimeRate = 1f;
            return;
        }
        bool input = p.InControl && Input.Aim.Value.Length() < 0.3f && !Input.Jump && !Input.Dash && !Input.CrouchDash && !Input.Grab;
        Engine.TimeRate = Calc.Approach(Engine.TimeRate, input ? 0.1f : 1f, 0.1f);
    }
}