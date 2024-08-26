using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

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

public class Shield : Entity {
    private bool breaking;
    private int flashAmount;

    public Shield(int flashes) {
        Add(new Image(GFX.Game["objects/GameHelper/shield_bubble"]) {
            RenderPosition = new Vector2(-16, -22)
        });
        Depth = -9999999;
        flashAmount = flashes;
    }

    private static PlayerDeadBody onDeath(On.Celeste.Player.orig_Die orig, Player p, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
        if(!evenIfInvincible && GameHelper.Session.PlayerHasShield) {
            ShieldTrigger.Shield.Break();
            return null;
        } else {
            return orig(p, direction, evenIfInvincible, registerDeathInStats);
        }
    }

    public void Break() {
        if(!breaking) {
            breaking = true;
            Add(new Coroutine(breakRoutine()));
        }
    }

    private IEnumerator breakRoutine() {
        Logger.Info("GameHelper", flashAmount.ToString());
        Audio.Play("event:/GameHelper/shield/shield");
        for(int i = 0; i < flashAmount; i++) {
            Visible = false;
            yield return 0.1f;
            Visible = true;
            yield return 0.1f;
        }
        RemoveSelf();
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        GameHelper.Session.PlayerHasShield = false;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        GameHelper.Session.PlayerHasShield = true;
    }

    public override void Render() {
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(SceneAs<Level>().Transitioning || p == null) return;
        Position = p.Center + (p.Facing == Facings.Right ? -Vector2.UnitX : Vector2.Zero);
        base.Render();
    }

    public static void Hook() {
        On.Celeste.Player.Die += onDeath;
    }

    public static void Unhook() {
        On.Celeste.Player.Die -= onDeath;
    }
}