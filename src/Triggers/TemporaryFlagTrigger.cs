using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/TemporaryFlagTrigger")]
public class TemporaryFlagTrigger : Trigger {
    private readonly string flag;
    private readonly bool invert;

    public TemporaryFlagTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        flag = data.Attr("flag");
        invert = data.Bool("invert");
    }

    public override void OnEnter(Player p) {
        SceneAs<Level>().Session.SetFlag(flag, !invert);
    }

    public override void Removed(Scene scene) {
        SceneAs<Level>().Session.SetFlag(flag, invert);
        base.Removed(scene);
    }

    public override void SceneEnd(Scene scene) {
        SceneAs<Level>().Session.SetFlag(flag, invert);
        base.SceneEnd(scene);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if (flag.Length == 0) {
            Logger.Warn("GameHelper", "Temporary Flag Trigger flag empty in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
    }
}