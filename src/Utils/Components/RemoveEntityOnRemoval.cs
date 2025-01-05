using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class RemoveEntityOnRemoval(Entity toRemove) : Component(false, false) {
    private readonly Entity toRemove = toRemove;

    public override void EntityRemoved(Scene scene) {
        toRemove.RemoveSelf();
        Logger.Info("GameHelper", "removed");
        base.EntityRemoved(scene);
    }
}