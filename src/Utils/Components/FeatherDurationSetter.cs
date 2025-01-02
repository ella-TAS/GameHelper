using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class FeatherDurationSetter : Component {
    private readonly float duration;
    private readonly Color flyColor;

    public FeatherDurationSetter(float duration, Color flyColor) : base(false, false) {
        this.duration = duration;
        this.flyColor = flyColor;
    }

    public float getDuration() {
        return duration;
    }

    public Color getColor() {
        return flyColor;
    }
}