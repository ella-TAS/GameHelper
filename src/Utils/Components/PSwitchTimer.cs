using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class PSwitchTimer : Component {
    private static Color StartColor, EndColor;
    private readonly string flag;
    private readonly float duration;
    private float timer;
    private bool soundPlayed;

    public PSwitchTimer(string flag, float duration) : base(true, true) {
        StartColor = new Color(97, 255, 85);
        EndColor = new Color(208, 28, 23);

        this.flag = flag;
        this.duration = timer = duration;
    }

    public override void Update() {
        timer -= Engine.DeltaTime;
        if (timer <= 1f && !soundPlayed) {
            Audio.Play("event:/GameHelper/p_switch/p_switch_timer");
            soundPlayed = true;
        }
        if (timer <= 0f) {
            GameHelper.Session.PSwitchTimers.Remove(flag);
            RemoveSelf();
        }
    }

    public override void Added(Entity entity) {
        base.Added(entity);
        GameHelper.Session.PSwitchTimers ??= new Dictionary<string, PSwitchTimer>();
        if (GameHelper.Session.PSwitchTimers.TryGetValue(flag, out PSwitchTimer p)) {
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
        if (SceneAs<Level>().Tracker.GetEntity<Player>() is Player p) {
            Vector2 bar = p.Position - new Vector2(6f, 20f);
            Draw.Rect(bar - Vector2.UnitY, 12, 4, Color.Black);
            Draw.Rect(bar - Vector2.UnitX, 14, 2, Color.Black);
            Draw.Rect(bar, (int) (12f * timer / duration + 0.99999f), 2, Util.ColorInterpolate(EndColor, StartColor, timer / duration));
        }
    }
}