using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/PlayerStateFlag")]
public class PlayerStateFlag : Entity {
    private readonly string flag;
    private readonly int state;
    private readonly bool invert, dashAttack;

    public PlayerStateFlag(EntityData data, Vector2 levelOffset) {
        flag = data.Attr("flag");
        state = data.Int("state");
        invert = data.Bool("invert");
        dashAttack = data.Bool("dashAttack");
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {

            bool isState = (!dashAttack && state == p.StateMachine.State) || (dashAttack && p.DashAttacking);
            SceneAs<Level>().Session.SetFlag(flag, isState ^ invert);
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(flag?.Length == 0) {
            Logger.Warn("GameHelper", "PlayerStateFlag: no flag set");
            RemoveSelf();
        }
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        if(flag?.Length > 0) {
            (scene as Level).Session.SetFlag(flag, false);
        }
    }
}