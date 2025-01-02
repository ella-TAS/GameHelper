using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/ImmediateFeather")]
public class ImmediateFeather : FlyFeather {
    private readonly Color color = Color.OrangeRed;

    public ImmediateFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("singleUse")) {
        Depth = -1;
        sprite.Color = color;
        PlayerCollider pc = Get<PlayerCollider>();
        System.Action<Player> orig = pc.OnCollide;
        pc.OnCollide = p => {
            orig(p);
            if(data.Bool("startBoost") && p.Speed.Length() < 600f) {
                p.Speed *= 600f / p.Speed.Length();
            }
            if(!data.Bool("startBoost")) {
                p.Speed *= 1.2f;
            }
            p.starFlyTransforming = false;
            p.starFlyLastDir = p.Speed;
            p.starFlyTimer = data.Float("flightDuration");
            p.starFlySpeedLerp = 1f;
            p.Sprite.Play("starFly");
            p.Sprite.SetColor(color);
            p.Sprite.HairCount = 7;
            p.Hair.DrawPlayerSpriteOutline = true;
            p.RefillDash();
            p.RefillStamina();
        };
    }
}