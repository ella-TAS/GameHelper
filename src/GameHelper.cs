using Monocle;
using System;
using Celeste.Mod.GameHelper.Entities;
using Celeste.Mod.GameHelper.Entities.Controllers;
using Celeste.Mod.GameHelper.Entities.Feathers;
using Celeste.Mod.GameHelper.Entities.Wrappers;
using Celeste.Mod.GameHelper.Triggers;

namespace Celeste.Mod.GameHelper;

public class GameHelper : EverestModule {
    public static GameHelper Instance;
    public override Type SessionType => typeof(GameHelperSession);
    public static GameHelperSession Session => (GameHelperSession) Instance._Session;

    internal static SpriteBank SpriteBank;
    internal static Random Random;
    internal static bool CollabUtilsLoaded;

    public GameHelper() {
        Instance = this;
        Random = new Random(0);
    }

    public override void Load() {
        Logger.SetLogLevel("GameHelper", 0);
        CollabUtilsLoaded = Everest.Loader.DependencyLoaded(new() {
            Name = "CollabUtils2",
            Version = new Version(1, 8, 11)
        });
        FloatyJumpController.Hook();
        SaveSpeedFeather.Hook();
        SlowdownCobweb.Hook();
        Shield.Hook();
        DashMagnet.Load();
        FlagCollectBerry.Hook();
        Util.Load();
        EntityModifier.Hook();
    }

    public override void Unload() {
        FloatyJumpController.Unhook();
        SaveSpeedFeather.Unhook();
        SlowdownCobweb.Unhook();
        Shield.Unhook();
        DashMagnet.Unload();
        FlagCollectBerry.Unhook();
        EntityModifier.Unhook();
    }

    public override void LoadContent(bool firstLoad) {
        SpriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }
}