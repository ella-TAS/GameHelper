using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class EntityMoveComponent : Component {
    public Entity moveEntity;
    public Vector2? offset;
    public bool removeCascade;

    public EntityMoveComponent(Entity moveEntity, Vector2? offset = null, bool removeCascade = false) : base(true, true) {
        this.moveEntity = moveEntity;
        this.offset = offset;
        this.removeCascade = removeCascade;
    }

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
        if(removeCascade) {
            moveEntity.RemoveSelf();
        }
    }
}