using Celeste.Mod.GameHelper.Utils;
using Monocle;

namespace Celeste.Mod.GameHelper.Features;

public static class Commands {
    [Command("clear_area", "Collect all berries and mark the area as cleared. For testing purposes.")]
    public static void CmdCompleteArea() {
        Level level = (Level) Engine.Scene;
        Util.CollectBerries(level.Tracker.GetEntity<Player>());
        level.RegisterAreaComplete();
    }

    [Command("auto_save", "Auto save.")]
    public static void CmdAutoSave() {
        Level level = (Level) Engine.Scene;
        level.AutoSave();
    }
}