using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.CCPorts;

[CustomEntity("GameHelper/LoreTablet")]
public class LoreTabletTalker : Trigger {
    private readonly TalkComponent talker;
    private readonly string dialog, flag, activateSfx, loopSfx;
    private readonly bool onlyOnce, flagValue;

    public LoreTabletTalker(EntityData data, Vector2 offset) : base(data, offset) {
        Collider = new Hitbox(data.Width, data.Height);

        Vector2 drawAt = new(data.Width / 2, 0f);
        if (data.Nodes.Length != 0) {
            drawAt = data.Nodes[0] - data.Position;
        }
        Add(talker = new TalkComponent(new Rectangle(0, 0, data.Width, data.Height), drawAt, OnTalk));
        talker.PlayerMustBeFacing = false;

        dialog = data.Attr("dialog");
        flag = data.Attr("flag");
        onlyOnce = data.Bool("onlyOnce", false);
        flagValue = data.Bool("flagValue", true);
        activateSfx = data.Attr("activateSfx");
        loopSfx = data.Attr("loopSfx");
    }

    public void OnTalk(Player player) {
        Scene.Add(new LoreTabletCutscene(player, dialog, flag, flagValue, activateSfx, loopSfx));
        if (onlyOnce)
            RemoveSelf();
    }
}