namespace Celeste.Mod.GameHelper;

// filled in Features.Meta.LevelMetaHooks
public class GameHelperLevelMeta {
    public int AutoSaveInterval { get; set; } = 0;
}

public class GameHelperYaml {
    public GameHelperLevelMeta GameHelperMeta { get; set; } = new();
}