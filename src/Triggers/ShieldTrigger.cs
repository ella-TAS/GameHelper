using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ShieldTrigger")]
public class ShieldTrigger : Trigger {
    public static Shield Shield;
    private readonly bool enable;
    private readonly int flashes;

    public ShieldTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        enable = data.Bool("enable");
        flashes = data.Int("flashAmount");
    }

    public override void OnEnter(Player p) {
        Shield?.RemoveSelf();
        if(enable) {
            SceneAs<Level>().Add(Shield = new Shield(flashes));
        }
    }
}