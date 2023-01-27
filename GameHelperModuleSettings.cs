using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.GameHelper;

public class GameHelperModuleSettings : EverestModuleSettings {
    #region keyboard
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.G)]
    public ButtonBinding FlashlightButton { get; set; }
    #endregion
}