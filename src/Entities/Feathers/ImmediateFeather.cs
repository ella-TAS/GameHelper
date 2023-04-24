using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/ImmediateFeather")]
public class ImmediateFeather : FlyFeather {
    internal static bool CancelFeather;
    private Color color = Color.OrangeRed;

    public ImmediateFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("oneUse")) {
        base.Depth = -1;
        DynamicData.For(this).Get<Sprite>("sprite").SetColor(color);
        PlayerCollider pc = Get<PlayerCollider>();
        var orig = pc.OnCollide;
        pc.OnCollide = delegate (Player p) {
            CancelFeather = data.Bool("cancelFeather");
            orig(p);
            p.Speed *= data.Bool("cancelFeather") ? 600f / p.Speed.Length() : 1.2f;
            DynamicData playerData = DynamicData.For(p);
            playerData.Set("starFlyTransforming", false);
            playerData.Set("starFlyLastDir", p.Speed);
            playerData.Set("starFlyTimer", data.Float("flightDuration"));
            playerData.Set("starFlySpeedLerp", 1f);
            p.Sprite.Play("starFly");
            p.Sprite.SetColor(color);
            p.Sprite.HairCount = 7;
            p.Hair.DrawPlayerSpriteOutline = true;
            p.RefillDash();
            p.RefillStamina();
        };
    }

    private static int OnStarFlyUpdate(On.Celeste.Player.orig_StarFlyUpdate orig, Player p) {
        int state = orig(p);
        if(state != 19) {
            CancelFeather = false;
            return state;
        } else if(CancelFeather && p.Speed.Length() <= 190f) {
            CancelFeather = false;
            return 0;
        }
        return 19;
    }

    public static void Hook() {
        On.Celeste.Player.StarFlyUpdate += OnStarFlyUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.StarFlyUpdate -= OnStarFlyUpdate;
    }
}