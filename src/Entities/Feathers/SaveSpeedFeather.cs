using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/SaveSpeedFeather")]
public class SaveSpeedFeather : FlyFeather {
    private static float StoredSpeed;
    private static bool Redirect, hasLead;
    private Color color, flyColor;
    private Color colorN = Color.Aqua;
    private Color flyColorN = Color.SeaGreen;
    private Color colorR = Color.DarkRed;
    private bool isLead;

    public SaveSpeedFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("oneUse")) {
        base.Depth = -1;
        if(data.Bool("redirectSpeed")) {
            color = flyColor = colorR;
        } else {
            color = colorN;
            flyColor = flyColorN;
        }
        DynamicData.For(this).Get<Sprite>("sprite").SetColor(color);
        PlayerCollider pc = Get<PlayerCollider>();
        System.Action<Player> orig = pc.OnCollide;
        pc.OnCollide = (Player p) => {
            if(StoredSpeed == 0) {
                Redirect = data.Bool("redirectSpeed");
                StoredSpeed = 1.2f * (Redirect ? p.Speed.Length() : p.Speed.X);
            }
            orig(p);
            p.Sprite.SetColor(flyColor);
        };
    }

    public override void Update() {
        base.Update();
        if(isLead && StoredSpeed != 0) {
            if(Redirect) {
                SceneAs<Level>().Tracker.GetEntity<Player>()?.Sprite.SetColor(colorR);
            } else {
                SceneAs<Level>().Tracker.GetEntity<Player>()?.Sprite.SetColor(flyColorN);
            }
        }
    }

    private static void OnStarFlyEnd(On.Celeste.Player.orig_StarFlyEnd orig, Player p) {
        orig(p);
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
    }

    public static void Unhook() {
        On.Celeste.Player.StarFlyEnd -= OnStarFlyEnd;
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