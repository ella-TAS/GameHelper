using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/AutoSaveIntervalController")]
public class AutoSaveIntervalController : Entity {
    private static bool Triggered;

    private readonly int interval;

    public AutoSaveIntervalController(EntityData data, Vector2 levelOffset) {
        interval = data.Int("interval");
    }

    public override void Update() {
        base.Update();

        if (SceneAs<Level>().OnInterval(interval * 60)) {
            Triggered = true;
        }

        if (Triggered && SceneAs<Level>().Tracker.GetEntity<Player>() is Player player && player.JustRespawned) {
            SceneAs<Level>().AutoSave();
            Triggered = false;
        }
    }
}