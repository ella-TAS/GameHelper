using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Utils;

public class PSwitchTimer : Component {
    private static Color StartColor, EndColor;
    private readonly string flag;
    private readonly float duration;
    private float timer;

    public PSwitchTimer(string flag, float duration) : base(true, true) {
        StartColor = new Color(105, 152, 255);
        EndColor = new Color(56, 6, 207);

        this.flag = flag;
        this.duration = timer = duration;
    }

    public override void Update() {
        timer -= Engine.DeltaTime;
        if(timer <= 0f) {
            GameHelper.Session.PSwitchTimers.Remove(flag);
            RemoveSelf();
        }
    }

    public override void Added(Entity entity) {
        base.Added(entity);
        GameHelper.Session.PSwitchTimers ??= new Dictionary<string, PSwitchTimer>();
        if(GameHelper.Session.PSwitchTimers.TryGetValue(flag, out PSwitchTimer p)) {
            p.RemoveSelf();
            GameHelper.Session.PSwitchTimers.Remove(flag);
        }
        GameHelper.Session.PSwitchTimers.Add(flag, this);
        SceneAs<Level>().Session.SetFlag(flag);
    }

    public override void Removed(Entity entity) {
        SceneAs<Level>()?.Session.SetFlag(flag, false);
        base.Removed(entity);
    }

    public override void Render() {
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        Util.DrawCircle(p.Center, 15 * timer / duration, Util.ColorInterpolate(EndColor, StartColor, timer / duration));
    }
}