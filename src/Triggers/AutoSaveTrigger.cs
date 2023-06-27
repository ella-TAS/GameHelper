using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/AutoSaveTrigger")]
public class AutoSaveTrigger : Trigger {
    public AutoSaveTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) { }

    public override void OnEnter(Player p) {
        SceneAs<Level>().AutoSave();
        RemoveSelf();
    }
}