using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ExitCollabLevelTrigger")]
public class ExitCollabLevelTrigger : Trigger {
    private readonly EntityData data;
    private readonly Vector2 levelOffset;
    private readonly float delay, timeRateWait;
    private readonly bool addHeartTrigger;
    private readonly string flag;
    private TimeRateModifier timeRateModifier;

    public ExitCollabLevelTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        delay = data.Float("delay");
        timeRateWait = data.Float("timeRate");
        addHeartTrigger = data.Bool("addHeartTrigger");
        flag = data.Attr("flag");
        this.data = data;
        this.levelOffset = levelOffset;
        Add(timeRateModifier = new TimeRateModifier(1f));
    }

    public override void OnStay(Player player) {
        if (!Util.GetFlag(flag, Scene, true)) {
            return;
        }
        Collidable = false;
        Add(new Coroutine(routineExit(player)));
    }

    private IEnumerator routineExit(Player p) {
        Level level = SceneAs<Level>();
        level.CanRetry = false;
        timeRateModifier.Multiplier = timeRateWait;
        Util.CollectBerries(p);
        yield return delay;
        timeRateModifier.Multiplier = 1f;
        if (p.Dead) {
            yield return float.MaxValue;
        }
        Tag = Tags.FrozenUpdate;
        level.Frozen = true;
        level.PauseLock = true;
        SceneAs<Level>().RegisterAreaComplete();

        //collab utils endscreen
        yield return new SwapImmediately(CollabUtils2Imports.DisplayCollabMapEndScreenIfEnabled());
        CollabUtils2Imports.TriggerReturnToLobby();
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if (!GameHelper.CollabUtilsLoaded) {
            Logger.Warn("GameHelper", "ExitCollabLevelTrigger: CollabUtils2 not found");
            RemoveSelf();
        } else if (addHeartTrigger) {
            SceneAs<Level>().Add(new RegisterHeartTrigger(data, levelOffset));
        }
    }
}