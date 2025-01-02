using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/PlayerStateFlag")]
public class PlayerStateFlag : Entity {
    private readonly string flag, stateName;
    private readonly int state;
    private readonly bool invert, dashAttack, useStateName, debug;

    public PlayerStateFlag(EntityData data, Vector2 levelOffset) {
        useStateName = data.Bool("useStateName");
        if(useStateName) {
            stateName = data.Attr("state");
        } else {
            state = data.Int("state");
        }

        flag = data.Attr("flag");
        invert = data.Bool("invert");
        dashAttack = data.Bool("dashAttack");
        debug = data.Bool("debug");
        Depth = -1;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            if(debug) {
                Logger.Info("GameHelper", p.StateMachine.State.ToString() + " - \"" + p.StateMachine.GetStateName(p.StateMachine.State) + "\"");
            }
            bool isState =
                (!dashAttack && !useStateName && state == p.StateMachine.State) ||
                (!dashAttack && useStateName && stateName.Equals(p.StateMachine.GetStateName(p.StateMachine.State))) ||
                (dashAttack && p.DashAttacking);
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
        SceneAs<Level>().Session.SetFlag(flag, false);
        base.Removed(scene);
    }
}
