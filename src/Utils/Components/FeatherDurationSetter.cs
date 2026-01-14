using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class FeatherDurationSetter(float duration, Color flyColor, bool legacy) : Component(false, false) {
    private readonly float duration = duration;
    private readonly Color flyColor = flyColor;
    public readonly bool legacy = legacy;

    public float getDuration() {
        return duration;
    }

    public Color getColor() {
        return flyColor;
    }
}