using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/FlagZoneTrigger")]
public class FlagZoneTrigger : Trigger {
    private readonly string flag;

    public FlagZoneTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        flag = data.Attr("flag");
    }

    public override void OnEnter(Player p) {
        SceneAs<Level>().Session.SetFlag(flag, true);
    }

    public override void OnLeave(Player p) {
        SceneAs<Level>().Session.SetFlag(flag, false);
    }
}