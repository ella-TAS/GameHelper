using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/AutoSaveIntervalController")]
public class AutoSaveIntervalController : Entity {
    private readonly int interval;

    public AutoSaveIntervalController(EntityData data, Vector2 levelOffset) {
        interval = data.Int("interval");
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        if (GameHelper.LevelMeta == null || GameHelper.LevelMeta.AutoSaveInterval == 0) {
            GameHelper.LevelMeta ??= new();
            GameHelper.LevelMeta.AutoSaveInterval = interval;
        } else {
            Logger.Warn("GameHelper", $"AutoSaveIntervalController: Not replacing previously set interval of {GameHelper.LevelMeta.AutoSaveInterval} min");
        }

        RemoveSelf();
    }
}