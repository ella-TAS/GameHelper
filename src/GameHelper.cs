using Celeste.Mod.GameHelper.Entities;
using Celeste.Mod.GameHelper.Entities.Controllers;
using Celeste.Mod.GameHelper.Entities.Feathers;
using Celeste.Mod.GameHelper.Entities.Wrappers;
using Celeste.Mod.GameHelper.Utils;
using Monocle;
using MonoMod.ModInterop;
using System;

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
        ModInteropManager.ModInterop(typeof(GameHelperExports));

        Logger.SetLogLevel("GameHelper", 0);
        CollabUtilsLoaded = Everest.Loader.DependencyLoaded(new() {
            Name = "CollabUtils2",
            Version = new Version(1, 10, 14)
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
        EntityIdApplier.Hook();
        MiscHooks.Hook();

        PlayerShadowController.resetBindings();
        FlashlightController.resetBindings();
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
        EntityIdApplier.Unhook();
        MiscHooks.Unhook();
    }

    public override void LoadContent(bool firstLoad) {
        SpriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }
}