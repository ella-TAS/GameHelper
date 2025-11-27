using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/SuperHotController")]
public class SuperHotController : Entity {
    private TimeRateModifier timeRateModifier;

    public SuperHotController() {
        Depth = -1;
        Add(timeRateModifier = new TimeRateModifier(1f));
    }

    public override void Update() {
        base.Update();
        if (SceneAs<Level>().Tracker.GetEntity<Player>() is Player p) {
            bool input = p.InControl && Input.Aim.Value.Length() < 0.3f && !Input.Jump && !Input.Dash && !Input.CrouchDash && !Input.Grab;
            timeRateModifier.Multiplier = Calc.Approach(timeRateModifier.Multiplier, input ? 0.1f : 1f, 0.1f);
        }
    }

    private static void OnDeath(Player p) {
        p.SceneAs<Level>().Entities.FindAll<SuperHotController>().ForEach(e => e.RemoveSelf());
    }

    public static void Hook() {
        Everest.Events.Player.OnDie += OnDeath;
    }

    public static void Unhook() {
        Everest.Events.Player.OnDie -= OnDeath;
    }
}