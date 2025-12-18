namespace Celeste.Mod.GameHelper.Features.Meta;

public class AutoSaveInterval {
    internal static bool Triggered;

    private static void OnLevelUpdate(On.Celeste.Level.orig_Update orig, Level level) {
        orig(level);

        if (GameHelper.LevelMeta?.AutoSaveInterval is int interval && interval > 0) {
            if (level.OnInterval(interval * 60)) {
                Triggered = true;
            }

            if (Triggered && level.Tracker.GetEntity<Player>() is Player player && player.JustRespawned) {
                Logger.Info("GameHelper", $"Auto Saving after death [interval: {interval} min]");
                level.AutoSave();
                Triggered = false;
            }
        }
    }

    public static void Hook() {
        On.Celeste.Level.Update += OnLevelUpdate;
    }

    public static void Unhook() {
        On.Celeste.Level.Update -= OnLevelUpdate;
    }
}