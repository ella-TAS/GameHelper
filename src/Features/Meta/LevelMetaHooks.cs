namespace Celeste.Mod.GameHelper.Features.Meta;

public static class LevelMetaHooks {
    private static GameHelperLevelMeta TryGetMetadata(Session session) {
        if (!Everest.Content.TryGet($"Maps/{session.MapData.Filename}.meta", out ModAsset asset)) return null;
        if (!(asset?.PathVirtual?.StartsWith("Maps") ?? false)) return null;
        if (!(asset?.TryDeserialize(out GameHelperYaml meta) ?? false)) return null;
        return meta?.GameHelperMeta;
    }

    private static void OnLoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader) {
        GameHelper.LevelMeta = TryGetMetadata(level.Session);
        AutoSaveInterval.Triggered = false;
    }

    private static void OnExitLevel(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow) {
        GameHelper.LevelMeta = null;
        AutoSaveInterval.Triggered = false;
    }

    public static void Hook() {
        Everest.Events.Level.OnLoadLevel += OnLoadLevel;
        Everest.Events.Level.OnExit += OnExitLevel;
    }

    public static void Unhook() {
        Everest.Events.Level.OnLoadLevel -= OnLoadLevel;
        Everest.Events.Level.OnExit -= OnExitLevel;
    }
}