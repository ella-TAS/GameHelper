using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ShieldTrigger")]
public class ShieldTrigger : Trigger {
    private readonly bool enable;
    private readonly int flashes;

    public ShieldTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        enable = data.Bool("enable");
        flashes = data.Int("flashAmount");
    }

    public override void OnEnter(Player p) {
        p.Scene.Entities.FindAll<Shield>().ForEach(s => s.RemoveSelf());
        if (enable) {
            SceneAs<Level>().Add(new Shield(flashes));
        }
    }
}