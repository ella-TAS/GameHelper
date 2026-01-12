using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/SaveSpeedFeather")]
public class SaveSpeedFeather : FlyFeather {
    private readonly Color spriteColor, flyColor;
    private readonly Color colorN = Color.Aqua;
    private readonly Color flyColorN = Color.SeaGreen;
    private readonly Color colorR = Color.DarkRed;

    public SaveSpeedFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("singleUse")) {
        Depth = -1;
        bool redirect = data.Bool("redirectSpeed");
        if (redirect) {
            spriteColor = flyColor = colorR;
        } else {
            spriteColor = colorN;
            flyColor = flyColorN;
        }
        sprite.Color = spriteColor;
        PlayerCollider pc = Get<PlayerCollider>();
        System.Action<Player> orig = pc.OnCollide;
        pc.OnCollide = p => {
            // conserve first speed storage, but replace duration setter
            if (p.Get<FeatherSpeedStorage>() == null) {
                p.Add(new FeatherSpeedStorage(1.2f * (redirect ? p.Speed.Length() : p.Speed.X), redirect));
            }
            orig(p);
            p.Sprite.SetColor(flyColor);
            p.Components.RemoveAll<FeatherDurationSetter>();
            p.Add(new FeatherDurationSetter(data.Float("flightDuration"), flyColor));
        };
    }

    private static void PlayerUpdate(Player p) {
        if (p.Get<FeatherDurationSetter>() is { } comp) {
            p.Sprite.SetColor(comp.getColor());
        }
    }

    private static IEnumerator OnStarFlyCoroutine(On.Celeste.Player.orig_StarFlyCoroutine orig, Player p) {
        IEnumerator origEnum = orig(p).SafeEnumerate();
        while (origEnum.MoveNext()) {
            if (p.Sprite.HairCount == 7 && p.Get<FeatherDurationSetter>() is FeatherDurationSetter comp) {
                p.starFlyTimer = comp.getDuration();
                p.Sprite.SetColor(comp.getColor());
                comp.RemoveSelf();
            }

            yield return origEnum.Current;
        }
    }

    private static void OnStarFlyEnd(On.Celeste.Player.orig_StarFlyEnd orig, Player p) {
        orig(p);
        p.Components.RemoveAll<FeatherDurationSetter>();
        if (p.Get<FeatherSpeedStorage>() is { } speed) {
            if (speed.getRedirect()) {
                p.Speed = p.Speed.SafeNormalize() * speed.getSpeed();
            } else {
                p.Speed.X = speed.getSpeed();
            }
            speed.RemoveSelf();
        }
    }

    public static void Hook() {
        On.Celeste.Player.StarFlyEnd += OnStarFlyEnd;
        On.Celeste.Player.StarFlyCoroutine += OnStarFlyCoroutine;
        Everest.Events.Player.OnAfterUpdate += PlayerUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.StarFlyEnd -= OnStarFlyEnd;
        On.Celeste.Player.StarFlyCoroutine -= OnStarFlyCoroutine;
        Everest.Events.Player.OnAfterUpdate -= PlayerUpdate;
    }
}