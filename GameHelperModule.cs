using System;
using Monocle;
using Celeste.Mods.GameHelper.Entities;

namespace Celeste.Mod.GameHelper;

public class GameHelperModule : EverestModule {
    public static GameHelperModule Instance;
    private static SpriteBank spriteBank;
    public override Type SettingsType => typeof(GameHelperModuleSettings);
    public static GameHelperModuleSettings Settings => (GameHelperModuleSettings) Instance._Settings;

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