using Celeste.Mod.GameHelper.Entities.Controllers;
using MonoMod.ModInterop;

namespace Celeste.Mod.GameHelper;

[ModExportName("GameHelper")]
public static class GameHelperExports {
    public static void AddFlashlightBinding(string levelSID, ButtonBinding binding) {
        FlashlightController.addBinding(levelSID, binding);
    }

    public static void ResetFlashlightBindings() {
        FlashlightController.resetBindings();
    }

    public static void AddPlayerShadowBinding(string levelSID, ButtonBinding binding) {
        PlayerShadowController.addBinding(levelSID, binding);
    }

    public static void ResetPlayerShadowBindings() {
        PlayerShadowController.resetBindings();
    }
}