using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityMoverFollow")]
public class EntityMoverFollow : EntityMover {

    public EntityMoverFollow(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {

    }

    public override void Awake(Scene scene) {
        target = FindNearest(Position, onlyType);
        if(target == null) {
            ComplainEntityNotFound("Generic Entity Mover");
            return;
        }
        base.Awake(scene);
    }
}