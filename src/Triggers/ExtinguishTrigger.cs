using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Entities.Controllers;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ExtinguishTrigger")]
public class ExtinguishTrigger : Trigger {
    public ExtinguishTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) { }

    public override void OnStay(Player p) {
        if(!SceneAs<Level>().Transitioning) {
            SceneAs<Level>().Tracker.GetEntity<RoasterController>()?.ResetTimer();
        }
    }
}