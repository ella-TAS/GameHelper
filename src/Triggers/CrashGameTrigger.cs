using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils.Exceptions;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/CrashGameTrigger")]
public class CrashGameTrigger : Trigger {
    private bool entered;
    private readonly string message;
    private readonly bool save;

    public CrashGameTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        message = data.Attr("message");
        save = data.Bool("saveGame");
    }

    public override void OnEnter(Player p) {
        if (!entered) {
            entered = true;
            if (save) {
                SceneAs<Level>().AutoSave();
                Add(new Coroutine(routineCrash()));
            } else {
                throw new TrollException(message);
            }
        }
    }

    private IEnumerator routineCrash() {
        while (SceneAs<Level>().IsAutoSaving()) {
            yield return null;
        }
        throw new TrollException(message);
    }
}