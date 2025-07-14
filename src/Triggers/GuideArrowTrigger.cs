using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/GuideArrowTrigger")]

public class GuideArrowTrigger : Trigger {
    private readonly bool onlyOnce;
    private readonly IEnumerable<Vector2> nodes;
    private readonly float duration;

    public GuideArrowTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        onlyOnce = data.Bool("onlyOnce");
        duration = data.Float("duration");
        nodes = data.Nodes.Select(v => v + levelOffset);
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);

        player.Components.RemoveAll<PlayerGuideArrow>();

        int i = 0;
        foreach(Vector2 pos in nodes) {
            player.Add(new PlayerGuideArrow(pos, duration, i));
            i++;
        }

        if(onlyOnce) {
            RemoveSelf();
        }
    }
}