using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/ShieldController")]
public class ShieldController : Entity {
    public static bool HasShield, Breaking, RenderBubble;

    public ShieldController(EntityData data, Vector2 levelOffset) {
        Sprite sprite = GameHelper.SpriteBank.Create("shield");
        sprite.RenderPosition = new Vector2(-16, -22);
        Add(sprite);
        base.Depth = -9999999;
    }

    private static PlayerDeadBody onDeath(On.Celeste.Player.orig_Die orig, Player p, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
        if(HasShield) {
            if(!Breaking) {
                Breaking = true;
                p.Add(new Coroutine(routineBreak()));
            }
            return null;
        } else {
            return orig(p, direction, evenIfInvincible, registerDeathInStats);
        }
    }

    private static IEnumerator routineBreak() {
        for(int i = 0; i < 3; i++) {
            RenderBubble = false;
            yield return 0.1f;
            RenderBubble = true;
            yield return 0.1f;
        }
        RenderBubble = false;
        HasShield = false;
    }

    public static void Hook() {
        On.Celeste.Player.Die += onDeath;
    }

    public static void Unhook() {
        On.Celeste.Player.Die -= onDeath;
    }

    public override void Update() {
        base.Update();
        Visible = RenderBubble;
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p != null) {
            Position = p.Center + (p.Facing == Facings.Right ? -Vector2.UnitX : Vector2.Zero);
        }
    }

    public override void Render() {
        if(!SceneAs<Level>().Transitioning) {
            base.Render();
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        HasShield = true;
        Breaking = false;
        RenderBubble = true;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        HasShield = false;
    }
}