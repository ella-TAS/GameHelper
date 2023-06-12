using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/FireTrap")]
public class FireTrap : Entity {
    private bool activated, deadly;
    private readonly float delay;

    public FireTrap(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        delay = data.Float("delay");
        base.Collider = new Hitbox(8, 64, 0, -56);
        Add(new PlayerCollider(onPlayer));
    }

    private void onPlayer(Player p) {
        if(!activated) {
            activated = true;
            Add(new Coroutine(routineActivation()));
        }
        if(deadly) {
            p.Die(-Vector2.UnitY);
        }
    }

    private IEnumerator routineActivation() {
        yield return delay;
        deadly = true;
        yield return 1f;
        deadly = activated = false;
    }
}