using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Celeste.Mod.CollabUtils2;
using Celeste.Mod.CollabUtils2.UI;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ExitCollabLevelTrigger")]
public class ExitCollabLevelTrigger : Trigger {
    private readonly EntityData data;
    private readonly Vector2 levelOffset;
    private readonly float delay, timeRateWait;
    private readonly bool addHeartTrigger;
    private readonly string flag;

    public ExitCollabLevelTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        delay = data.Float("delay");
        timeRateWait = data.Float("timeRate");
        addHeartTrigger = data.Bool("addHeartTrigger");
        flag = data.Attr("flag");
        this.data = data;
        this.levelOffset = levelOffset;
    }

    public override void OnStay(Player player) {
        if(flag != "" && !SceneAs<Level>().Session.GetFlag(flag)) {
            return;
        }
        Collidable = false;
        Add(new Coroutine(routineExit(player)));
    }

    private IEnumerator routineExit(Player p) {
        Level level = SceneAs<Level>();
        level.CanRetry = false;
        Engine.TimeRate = timeRateWait;
        collectBerries(p);
        yield return delay;
        Engine.TimeRate = 1f;
        if(p.Dead) {
            yield return float.MaxValue;
        }
        Tag = Tags.FrozenUpdate;
        level.Frozen = true;
        level.PauseLock = true;
        SceneAs<Level>().RegisterAreaComplete();

        //collab utils endscreen
        if(CollabModule.Instance.Settings.DisplayEndScreenForAllMaps) {
            Scene.Add(new AreaCompleteInfoInLevel());
            yield return 0.5f;
            while(!Input.MenuConfirm.Pressed && !Input.MenuCancel.Pressed) {
                yield return null;
            }
        } else {
            float endscreenTime = 0f;
            while(!Input.MenuConfirm.Pressed && !Input.MenuCancel.Pressed && endscreenTime <= 1f) {
                yield return null;
                endscreenTime += Engine.DeltaTime;
            }
        }
        level.DoScreenWipe(wipeIn: false, delegate {
            Engine.Scene = new LevelExitToLobby(LevelExit.Mode.Completed, level.Session);
        });
    }

    private void collectBerries(Player p) {
        List<IStrawberry> berries = new();
        ReadOnlyCollection<Type> berryTypes = StrawberryRegistry.GetBerryTypes();
        foreach(Follower follower in p.Leader.Followers) {
            if(berryTypes.Contains(follower.Entity.GetType()) && follower.Entity is IStrawberry s) {
                berries.Add(s);
            }
        }
        foreach(IStrawberry berry in berries) {
            berry.OnCollect();
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(!GameHelper.CollabUtilsLoaded) {
            Logger.Warn("GameHelper", "ExitCollabLevelTrigger: CollabUtils2 not found");
            RemoveSelf();
        } else if(addHeartTrigger) {
            SceneAs<Level>().Add(new RegisterHeartTrigger(data, levelOffset));
        }
    }
}