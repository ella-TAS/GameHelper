using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class FeatherSpeedStorage(float speed, bool redirect) : Component(false, false) {
    private readonly float speed = speed;
    private readonly bool redirect = redirect;

    public float getSpeed() {
        return speed;
    }

    public bool getRedirect() {
        return redirect;
    }
}