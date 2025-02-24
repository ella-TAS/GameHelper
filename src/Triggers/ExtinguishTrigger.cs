using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Entities.Controllers;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ExtinguishTrigger")]
public class ExtinguishTrigger(EntityData data, Vector2 levelOffset) : Trigger(data, levelOffset) {
    public override void OnStay(Player p) {
        if(!SceneAs<Level>().Transitioning) {
            SceneAs<Level>().Tracker.GetEntity<RoasterController>()?.ResetTimer();
        }
    }
}