using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities.Feathers;

[CustomEntity("GameHelper/SaveSpeedFeather")]
public class SaveSpeedFeather : FlyFeather {
    private static float StoredSpeed;
    private Color color = Color.Aqua;
    private Color flyColor = Color.SeaGreen;

    public SaveSpeedFeather(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Bool("shielded"), data.Bool("oneUse")) {
        base.Depth = -1;
        DynamicData.For(this).Get<Sprite>("sprite").SetColor(color);
        PlayerCollider pc = Get<PlayerCollider>();
        var orig = pc.OnCollide;
        pc.OnCollide = delegate (Player p) {
            if(StoredSpeed == 0) {
                StoredSpeed = 1.2f * p.Speed.X;
            }
            orig(p);
            p.Sprite.SetColor(flyColor);
        };
    }

    public override void Update() {
        base.Update();
        if(StoredSpeed != 0) {
            SceneAs<Level>().Tracker.GetEntity<Player>()?.Sprite.SetColor(flyColor);
        }
    }

    private static void OnStarFlyEnd(On.Celeste.Player.orig_StarFlyEnd orig, Player p) {
        orig(p);
        if(StoredSpeed != 0) {
            p.Speed.X = StoredSpeed;
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
    }
}