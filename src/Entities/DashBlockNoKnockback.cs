using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/DashBlockNoKnockback")]
public class DashBlockNoKnockback : DashBlock {
    public DashBlockNoKnockback(EntityData data, Vector2 levelOffset, EntityID id)
    : base(data, levelOffset, id) {
        OnDashCollide = delegate (Player p, Vector2 dir) {
            base.Break(p.Center, dir, true, true);
            return DashCollisionResults.Ignore;
        };
    }
}