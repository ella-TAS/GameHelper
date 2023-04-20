using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SuperHotController")]
public class SuperHotController : Entity {
    public SuperHotController(EntityData data, Vector2 levelOffset) {
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p == null) {
            Engine.TimeRate = 1f;
            return;
        }
        bool input = p.InControl & Input.Aim.Value.Length() < 0.3f & !Input.Jump & !Input.Dash & !Input.CrouchDash & !Input.Grab;
        Engine.TimeRate = Calc.Approach(Engine.TimeRate, input ? 0.05f : 1f, 0.1f);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(SceneAs<Level>().Entities.AmountOf<SuperHotController>() > 1) {
            Logger.Log("GameHelper", "WARN – Multiple SuperHotControllers in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
    }
}