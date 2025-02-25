using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[Tracked]
[CustomEntity("GameHelper/SlowdownCobweb")]
public class SlowdownCobweb : Entity {
    private Sprite sprite;
    private readonly int width, height;
    private bool decaying;

    public SlowdownCobweb(Vector2 position) : base(position) {
        width = height = 8;
    }

    public SlowdownCobweb(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        width = data.Width;
        height = data.Height;
    }

    public void Decay() {
        if(!decaying) {
            decaying = true;
            sprite.Play("decay");
            Add(new Coroutine(routineDecay()));
        }
    }

    private IEnumerator routineDecay() {
        yield return 0.5f;
        RemoveSelf();
    }

    private static void OnPlayerUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);
        SlowdownCobweb nearestWeb = null;
        float minDistance = float.MaxValue;
        foreach(SlowdownCobweb c in p.CollideAll<SlowdownCobweb>()) {
            if(Vector2.Distance(p.Center, c.Center) < minDistance) {
                nearestWeb = c;
                minDistance = Vector2.Distance(p.Center, c.Center);
            }
        }
        nearestWeb?.Decay();
        if(nearestWeb != null && !p.DashAttacking) {
            p.Speed.X = Calc.Approach(p.Speed.X, Input.Aim.Value.X * 10, 40);
            if(!p.OnGround() || p.Speed.Y != 0) {
                p.AutoJump = true;
                p.Speed.Y = Calc.Approach(p.Speed.Y, 0, 40);
                p.varJumpTimer = 0f;
            }
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(width == 8 && height == 8) {
            Depth = -1;
            Collider = new Hitbox(8, 8);
            Add(sprite = GameHelper.SpriteBank.Create("cobweb" + GameHelper.Random.Next(3)));
            return;
        }
        for(int w = 0; w < width; w += 8) {
            for(int h = 0; h < height; h += 8) {
                SceneAs<Level>().Add(new SlowdownCobweb(Position + new Vector2(w, h)));
            }
        }
        RemoveSelf();
    }

    public override void DebugRender(Camera camera) {
        if(decaying) {
            Collider.Render(camera, Color.Green);
        } else {
            base.DebugRender(camera);
        }
    }

    public static void Hook() {
        On.Celeste.Player.Update += OnPlayerUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.Update -= OnPlayerUpdate;
    }
}