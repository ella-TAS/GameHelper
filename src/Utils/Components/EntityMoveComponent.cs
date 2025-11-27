using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class EntityMoveComponent(Entity moveEntity, Vector2? offset = null, bool removeCascade = false) : Component(true, true) {
    public Entity moveEntity = moveEntity;
    public Vector2? offset = offset;
    public bool removeCascade = removeCascade;

    public override void Update() {
        base.Update();
        moveEntity.Position = Entity.Position + offset ?? Vector2.Zero;
    }

    public override void Added(Entity entity) {
        base.Added(entity);
        offset ??= moveEntity.Position - entity.Position;
    }

    public override void EntityRemoved(Scene scene) {
        base.EntityRemoved(scene);
        if (removeCascade) {
            moveEntity.RemoveSelf();
        }
    }
}