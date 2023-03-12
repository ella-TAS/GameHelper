using Monocle;
using System;

namespace Celeste.Mod.GameHelper;

public class GameHelperModule : EverestModule {
    private static SpriteBank spriteBank;
    internal static Random Random;
    internal static int BalloonCount;

    public GameHelperModule() {
        Random = new Random(0);
        BalloonCount = 0;
    }

    public override void Load() {
        Logger.SetLogLevel("GameHelper", 0);
    }

    public override void Unload() {}

    public override void LoadContent(bool firstLoad) {
        spriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }

    public static SpriteBank GetSpriteBank() {
        return spriteBank;
    }

    public static void IncreaseBalloon() {
        if(BalloonCount < 7) {
            BalloonCount++;
        }
    }
}