using Monocle;
using System;

namespace Celeste.Mod.GameHelper;

public class GameHelper : EverestModule {
    internal static SpriteBank SpriteBank;
    internal static Random Random;
    internal static int BalloonCount;

    public GameHelper() {
        Random = new Random(0);
        Logger.SetLogLevel("GameHelper", 0);
    }

    public override void Load() { }

    public override void Unload() { }

    public override void LoadContent(bool firstLoad) {
        SpriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }
}