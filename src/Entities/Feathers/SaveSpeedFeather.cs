using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System.Collections;
using Celeste.Mod.GameHelper.Utils.Components;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/SaveSpeedFeather")]
public class SaveSpeedFeather : FlyFeather {
    private static float StoredSpeed;
    private static bool Redirect, hasLead;
    private readonly Color color, flyColor;
    private readonly Color colorN = Color.Aqua;
    private readonly Color flyColorN = Color.SeaGreen;
    private readonly Color colorR = Color.DarkRed;
    private bool isLead;

    public SaveSpeedFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("singleUse")) {
        Depth = -1;
        if(data.Bool("redirectSpeed")) {
            color = flyColor = colorR;
        } else {
            color = colorN;
            flyColor = flyColorN;
        }
        sprite.Color = color;
        PlayerCollider pc = Get<PlayerCollider>();
        System.Action<Player> orig = pc.OnCollide;
        pc.OnCollide = p => {
            if(StoredSpeed == 0) {
                Redirect = data.Bool("redirectSpeed");
                StoredSpeed = 1.2f * (Redirect ? p.Speed.Length() : p.Speed.X);
            }
            orig(p);
            p.Sprite.SetColor(flyColor);
            p.starFlyTimer = data.Float("flightDuration");
            p.Components.RemoveAll<FeatherDurationSetter>();
            p.Add(new FeatherDurationSetter(data.Float("flightDuration"), flyColor));
        };
    }

    public override void Update() {
        base.Update();
        if(isLead && StoredSpeed != 0) {
            SceneAs<Level>().Tracker.GetEntity<Player>()?.Sprite.SetColor(Redirect ? colorR : flyColorN);
        }
    }

    private static IEnumerator OnStarFlyCoroutine(On.Celeste.Player.orig_StarFlyCoroutine orig, Player p) {
        IEnumerator origEnum = orig(p);
        while(origEnum.MoveNext()) {
            if(p.starFlyTimer == 2f) {
                FeatherDurationSetter comp = p.Get<FeatherDurationSetter>();
                if(comp != null) {
                    p.starFlyTimer = comp.getDuration();
                    p.Sprite.SetColor(comp.getColor());
                }
            }

            yield return origEnum.Current;
        }
    }

    private static void OnStarFlyEnd(On.Celeste.Player.orig_StarFlyEnd orig, Player p) {
        orig(p);
        p.Components.RemoveAll<FeatherDurationSetter>();
        if(StoredSpeed != 0) {
            if(Redirect) {
                p.Speed *= StoredSpeed / p.Speed.Length();
            } else {
                p.Speed.X = StoredSpeed;
            }
            StoredSpeed = 0;
        }
    }

    public static void Hook() {
        On.Celeste.Player.StarFlyEnd += OnStarFlyEnd;
        On.Celeste.Player.StarFlyCoroutine += OnStarFlyCoroutine;
    }

    public static void Unhook() {
        On.Celeste.Player.StarFlyEnd -= OnStarFlyEnd;
        On.Celeste.Player.StarFlyCoroutine -= OnStarFlyCoroutine;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        StoredSpeed = 0;
        hasLead = false;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(!hasLead) {
            hasLead = true;
            isLead = true;
        }
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        StoredSpeed = 0;
    }

    public override void SceneEnd(Scene scene) {
        base.SceneEnd(scene);
        StoredSpeed = 0;
    }
}