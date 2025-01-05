using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.GameHelper.Utils;

public class Shield : Entity {
    private bool breaking;
    private readonly int flashAmount;

    public Shield(int flashes) {
        Add(new Image(GFX.Game["objects/GameHelper/shield_bubble"]) {
            RenderPosition = new Vector2(-16, -22)
        });
        Depth = -9999999;
        flashAmount = flashes;
    }

    private static PlayerDeadBody OnDeath(On.Celeste.Player.orig_Die orig, Player p, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
        if(!evenIfInvincible && GameHelper.Session.PlayerHasShield) {
            p.Scene.Entities.FindAll<Shield>().ForEach(s => s.Break());
            return null;
        } else {
            return orig(p, direction, evenIfInvincible, registerDeathInStats);
        }
    }

    private void Break() {
        if(!breaking) {
            breaking = true;
            Add(new Coroutine(breakRoutine()));
        }
    }

    private IEnumerator breakRoutine() {
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

    public override void SceneEnd(Scene scene) {
        base.SceneEnd(scene);
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
        On.Celeste.Player.Die += OnDeath;
    }

    public static void Unhook() {
        On.Celeste.Player.Die -= OnDeath;
    }
}