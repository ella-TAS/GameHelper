using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/TimeFreezeController")]
public class TimeFreezeController : Entity {
    public TimeFreezeController(EntityData data, Vector2 levelOffset) {
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            if(p.JustRespawned && p.InControl && Input.Aim.Value.Length() < 0.3f
            && !Input.Jump && !Input.Dash && !Input.CrouchDash && !Input.Grab) {
                if(Engine.TimeRate > 0) {
                    Engine.TimeRate = 0;
                }
            } else if(Engine.TimeRate == 0) {
                Engine.TimeRate = 1;
                p.JustRespawned = false;
            }
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(SceneAs<Level>().Entities.AmountOf<TimeFreezeController>() > 1) {
            Logger.Log("GameHelper", "WARN – Multiple TimeFreezeControllers in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
    }
}