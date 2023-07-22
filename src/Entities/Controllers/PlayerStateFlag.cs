using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/PlayerStateFlag")]
public class PlayerStateFlag : Entity {
    private readonly string flag;
    private readonly int state;
    private readonly bool invert;

#pragma warning disable IDE0060, RCS1163
    public PlayerStateFlag(EntityData data, Vector2 levelOffset) {
        flag = data.Attr("flag");
        state = data.Int("state");
        invert = data.Bool("invert");
        base.Depth = -1;
    }
#pragma warning restore

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            bool isState = state == p.StateMachine.State;
            SceneAs<Level>().Session.SetFlag(flag, isState ^ invert);
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(flag?.Length == 0) {
            Logger.Log(LogLevel.Warn, "GameHelper", "PlayerStateFlag: no flag set");
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