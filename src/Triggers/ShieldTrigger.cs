using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/ShieldTrigger")]
public class ShieldTrigger : Trigger {
    public static Shield Shield;
    private int flashes;
    private bool enable;

    public ShieldTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        enable = data.Bool("enable");
        flashes = data.Int("flashAmount");
    }

    public override void OnEnter(Player p) {
        Shield.Enable(enable, flashes);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        scene.Add(Shield = new Shield());
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        Shield?.RemoveSelf();
    }
}

public class Shield : Entity {
    public static bool HasShield, Breaking, Show;
    public static int FlashAmount;

    public Shield() {
        Sprite sprite = GameHelper.SpriteBank.Create("shield");
        sprite.RenderPosition = new Vector2(-16, -22);
        Add(sprite);
        base.Depth = -9999999;
    }

    private static PlayerDeadBody onDeath(On.Celeste.Player.orig_Die orig, Player p, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
        if(!evenIfInvincible && HasShield) {
            if(!Breaking) {
                Breaking = true;
                //break the current shield
                ShieldTrigger.Shield.Add(new Coroutine(routineBreak()));
            }
            return null;
        } else {
            return orig(p, direction, evenIfInvincible, registerDeathInStats);
        }
    }

    private static IEnumerator routineBreak() {
        for(int i = 0; i < FlashAmount; i++) {
            Show = false;
            yield return 0.1f;
            Show = true;
            yield return 0.1f;
        }
        Show = false;
        HasShield = false;
        Breaking = false;
    }

    public void Enable(bool enable, int flashes) {
        Get<Coroutine>()?.RemoveSelf();
        Breaking = false;
        HasShield = Show = enable;
        FlashAmount = flashes;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        HasShield = Breaking = Show = false;
    }

    public override void Render() {
        if(Show && !SceneAs<Level>().Transitioning) {
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null) {
                Position = p.Center + (p.Facing == Facings.Right ? -Vector2.UnitX : Vector2.Zero);
                base.Render();
            }
        }
    }

    public static void Hook() {
        On.Celeste.Player.Die += onDeath;
    }

    public static void Unhook() {
        On.Celeste.Player.Die -= onDeath;
    }
}