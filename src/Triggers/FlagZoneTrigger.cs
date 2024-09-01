using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/FlagZoneTrigger")]
public class FlagZoneTrigger : Trigger {
    private readonly string flag;

    public FlagZoneTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        flag = data.Attr("flag");
    }

    public override void OnEnter(Player p) {
        SceneAs<Level>().Session.SetFlag(flag);
    }

    public override void OnLeave(Player p) {
        SceneAs<Level>().Session.SetFlag(flag, false);
    }

    public override void SceneEnd(Scene scene) {
        base.SceneEnd(scene);
        SceneAs<Level>().Session.SetFlag(flag, false);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(flag?.Length == 0) {
            Logger.Warn("GameHelper", "FlagZoneTrigger: no flag set");
            RemoveSelf();
        }
    }
}