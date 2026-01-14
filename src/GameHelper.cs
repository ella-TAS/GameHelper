using Celeste.Mod.GameHelper.Entities;
using Celeste.Mod.GameHelper.Entities.Controllers;
using Celeste.Mod.GameHelper.Entities.Feathers;
using Celeste.Mod.GameHelper.Entities.Wrappers;
using Celeste.Mod.GameHelper.Features;
using Celeste.Mod.GameHelper.Features.DebugMap;
using Celeste.Mod.GameHelper.Features.Meta;
using Celeste.Mod.GameHelper.Utils;
using Monocle;
using MonoMod.ModInterop;
using System;

namespace Celeste.Mod.GameHelper;

public class GameHelper : EverestModule {
    public static GameHelper Instance;
    public override Type SessionType => typeof(GameHelperSession);
    public static GameHelperSession Session => (GameHelperSession) Instance._Session;

    public static GameHelperLevelMeta LevelMeta;
    internal static SpriteBank SpriteBank;
    internal static Random Random;
    internal static bool CollabUtilsLoaded;

    public GameHelper() {
        Instance = this;
        Random = new Random(0);
    }

    public override void Load() {
        ModInteropManager.ModInterop(typeof(GameHelperExports));
        typeof(CollabUtils2Imports).ModInterop();

        Logger.SetLogLevel("GameHelper", 0);
        CollabUtilsLoaded = Everest.Loader.DependencyLoaded(new() {
            Name = "CollabUtils2",
            Version = new Version(1, 12, 0)
        });

        FloatyJumpController.Hook();
        SaveSpeedFeather.Hook();
        SlowdownCobweb.Hook();
        Shield.Hook();
        DashMagnet.Load();
        FlagCollectBerry.Hook();
        Util.Load();
        EntityModifier.Hook();
        PushBox.Hook();
        PSwitch.Hook();
        MarioMole.Hook();
        EntityRespriter.Hook();
        RopeSegment.Hook();
        MiscHooks.Hook();
        EntitySearch.Hook();
        ColorfulDebug.Hook();
        SuperHotController.Hook();
        LevelMetaHooks.Hook();
        AutoSaveInterval.Hook();
        Balloon.Hook();
        CustomBalloon.Hook();
    }

    public override void Unload() {
        FloatyJumpController.Unhook();
        SaveSpeedFeather.Unhook();
        SlowdownCobweb.Unhook();
        Shield.Unhook();
        DashMagnet.Unload();
        FlagCollectBerry.Unhook();
        EntityModifier.Unhook();
        PushBox.Unhook();
        PSwitch.Unhook();
        MarioMole.Unhook();
        EntityRespriter.Unhook();
        RopeSegment.Unhook();
        MiscHooks.Unhook();
        EntitySearch.Unhook();
        ColorfulDebug.Unhook();
        SuperHotController.Unhook();
        LevelMetaHooks.Unhook();
        AutoSaveInterval.Unhook();
        Balloon.Unhook();
        CustomBalloon.Unhook();
    }

    public override void LoadContent(bool firstLoad) {
        SpriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }
}