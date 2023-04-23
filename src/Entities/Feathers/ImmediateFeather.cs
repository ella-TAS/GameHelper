using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/ImmediateFeather")]
public class ImmediateFeather : FlyFeather {
    public ImmediateFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("oneUse")) {
        base.Depth = -1;
        PlayerCollider pc = Get<PlayerCollider>();
        var orig = pc.OnCollide;
        pc.OnCollide = delegate (Player p) {
            orig(p);
            p.Speed.X *= 1.2f;
            DynamicData playerData = DynamicData.For(p);
            playerData.Set("starFlyTransforming", false);
            playerData.Set("starFlyLastDir", p.Speed);
            playerData.Set("starFlyTimer", data.Float("flightDuration"));
            playerData.Set("starFlySpeedLerp", 1f);
            p.Sprite.Play("starFly");
            p.Sprite.Color = Color.OrangeRed;
            p.Sprite.HairCount = 5;
            p.Hair.DrawPlayerSpriteOutline = true;
            p.RefillDash();
            p.RefillStamina();
        };
    }
}