using Monocle;

namespace Celeste.Mod.GameHelper;

public class GameHelperModule : EverestModule {
    
    public static GameHelperModule Instance;
    private static SpriteBank spriteBank;
    
    public GameHelperModule() {
        Instance = this;
    }

    public override void Load() {}

    public override void Unload() {}

    public override void LoadContent(bool firstLoad) {
        spriteBank = new SpriteBank(GFX.Game, "Graphics/GameHelper/CustomSprites.xml");
    }

    public static SpriteBank getSpriteBank() {
        return spriteBank;
    }
}