using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/ImmediateFeather")]
public class ImmediateFeather : FlyFeather {
    private Color color = Color.OrangeRed;

    public ImmediateFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("oneUse")) {
        base.Depth = -1;
        DynamicData.For(this).Get<Sprite>("sprite").SetColor(color);
        PlayerCollider pc = Get<PlayerCollider>();
        System.Action<Player> orig = pc.OnCollide;
        pc.OnCollide = (Player p) => {
            orig(p);
            if(data.Bool("startBoost") && p.Speed.Length() < 600f) {
                p.Speed *= 600f / p.Speed.Length();
            }
            if(!data.Bool("startBoost")) {
                p.Speed *= 1.2f;
            }
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
}