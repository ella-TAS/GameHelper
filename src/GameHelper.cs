using Monocle;
using System;
using Celeste.Mod.GameHelper.Entities.Controllers;

namespace Celeste.Mod.GameHelper;

public class GameHelper : EverestModule {
    public static GameHelper Instance;
    public override Type SessionType => typeof(GameHelperSession);
    public GameHelperSession Session => _Session as GameHelperSession;

    internal static SpriteBank SpriteBank;
    internal static Random Random;

    public GameHelper() {
        Instance = this;
        Random = new Random(0);
        Logger.SetLogLevel("GameHelper", 0);
    }

    public override void Load() {
        FloatyJumpController.Load();
    }

    public override void Unload() {
        FloatyJumpController.Unload();
    }

    public override void LoadContent(bool firstLoad) {
        SpriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }
}