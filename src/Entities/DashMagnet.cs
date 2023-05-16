using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/DashMagnet")]
[Tracked]
public class DashMagnet : Entity {
    private static bool InsideMagnet;
    private static Vector2 Direction, Speed;
    private static Vector2[] RenderPoints;
    private Sprite sprite;
    private Vector2 startPosition;
    private bool inside, wasInside;

    public DashMagnet(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        base.Collider = new Circle(30, 8, 8);
        Add(sprite = GameHelper.SpriteBank.Create("dash_magnet"));
        Add(new PlayerCollider(onCollide));
    }

    private void onCollide(Player p) {
        if(p.StateMachine.State == 2) {
            if(!wasInside && !InsideMagnet) {
                //entered
                sprite.Play("flash");
                startPosition = p.Position;
            }
            if(!wasInside || Speed.Length() == 0) {
                if(!wasInside) {
                    Direction = (Center - p.Center).SafeNormalize();
                }
                Speed = Direction * p.Speed.Length();
                p.Position = startPosition;
            }
            p.DashDir = Direction;
            p.Speed = Speed * 1.2f;
            inside = true;
        }
    }

    public override void Update() {
        base.Update();
        if(inside && !wasInside) {
            wasInside = true;
            InsideMagnet = true;
        }
        if(!inside && wasInside) {
            wasInside = false;
            InsideMagnet = false;
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null) {
                p.StateMachine.ForceState(0);
            }
        }
        inside = false;
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        InsideMagnet = false;
    }

    private static IEnumerator DashCoroutine(On.Celeste.Player.orig_DashCoroutine orig, Player p) {
        IEnumerator origEnum = orig(p);
        while(origEnum.MoveNext()) {
            yield return origEnum.Current;
            if(origEnum.Current != null && InsideMagnet) {
                //magnet dash, cancelled by magnet
                yield return float.MaxValue;
            }
        }
    }

    public static void Load() {
        On.Celeste.Player.DashCoroutine += DashCoroutine;
    }

    public static void Unload() {
        On.Celeste.Player.DashCoroutine -= DashCoroutine;
    }

    public override void Render() {
        base.Render();
    }
}