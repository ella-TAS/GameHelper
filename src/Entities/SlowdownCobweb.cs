using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.GameHelper.Entities;

[Tracked]
[CustomEntity("GameHelper/SlowdownCobweb")]
public class SlowdownCobweb : Entity {
    internal static bool PlayerInWeb;
    private int width, height;
    private float decay; //max 0.5 s

    public SlowdownCobweb(Vector2 position) : base(position) {
        width = height = 8;
        base.Collider = new Hitbox(8, 8);
    }

    public SlowdownCobweb(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        width = data.Width;
        height = data.Height;
    }

    public void Decay() {
        decay += Engine.DeltaTime;
        if(decay >= 0.5f) {
            RemoveSelf();
        }
    }

    private static void OnPlayerUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        SlowdownCobweb nearestWeb = null;
        float minDistance = float.MaxValue;
        foreach(SlowdownCobweb c in p.CollideAll<SlowdownCobweb>()) {
            if(Vector2.Distance(p.Position, c.Position) < minDistance) {
                nearestWeb = c;
                minDistance = Vector2.Distance(p.Position, c.Position);
            }
        }
        if(nearestWeb != null) {
            nearestWeb.Decay();
            p.Speed.X = Calc.Approach(p.Speed.X, Input.Aim.Value.X * 10, 20);
            if(!p.OnGround() || p.Speed.Y != 0) {
                p.AutoJump = true;
                p.Speed.Y = 0;
                DynamicData.For(p).Set("varJumpTimer", 0);
            }
        }
    }


    public override void Added(Scene scene) {
        base.Added(scene);
        if(width == 8 && height == 8) {
            return;
        }
        for(int w = 0; w < width; w += 8) {
            for(int h = 0; h < height; h += 8) {
                SceneAs<Level>().Add(new SlowdownCobweb(Position + new Vector2(w, h)));
            }
        }
        RemoveSelf();
    }

    public static void Hook() {
        On.Celeste.Player.Update += OnPlayerUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.Update -= OnPlayerUpdate;
    }
}