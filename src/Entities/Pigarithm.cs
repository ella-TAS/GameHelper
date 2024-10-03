using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;
using Celeste.Mod.GameHelper.Utils;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Pigarithm")]
public class Pigarithm : Solid {
    private const float gravity = 7.5f;
    private const float fallCap = 160f;
    private readonly Sprite sprite;
    private readonly float speedX;
    private bool movingRight, resting;
    private readonly bool kill;
    private readonly string flag;

    public Pigarithm(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speedX = data.Float("speed");
        movingRight = data.Bool("startRight");
        kill = data.Bool("kill");
        flag = data.Attr("flag");
        Depth = -1;
        sprite = GameHelper.SpriteBank.Create(data.Attr("sprite"));
        sprite.RenderPosition = new Vector2(-8, 0);
        sprite.FlipY = data.Bool("flipSprite");
        Add(sprite);
    }

    public override void Update() {
        base.Update();

        //player kill check
        if(kill) {
            Player p = Scene.Tracker.GetEntity<Player>();
            if(p != null && (p.CollideCheck(this, p.Position + Vector2.UnitX) || p.CollideCheck(this, p.Position - Vector2.UnitX))) {
                p.Die((p.Center - Center).SafeNormalize());
            }
        }

        //x movement
        if(!resting && Util.GetFlag(flag, this)) {
            bool collided = MoveHCollideSolidsAndBounds(SceneAs<Level>(), (movingRight ? 1 : -1) * speedX * Engine.DeltaTime, thruDashBlocks: true);
            if(!collided) {
                foreach(SeekerBarrier s in SceneAs<Level>().Tracker.GetEntities<SeekerBarrier>()) {
                    if(s.CollideCheck(this)) {
                        collided = true;
                        MoveH((movingRight ? -1 : 1) * speedX * Engine.DeltaTime);
                        break;
                    }
                }
            }
            if(collided) {
                movingRight = !movingRight;
                sprite.Play("spin");
                Add(new Coroutine(routineRest()));
            }
        }
    }

    private IEnumerator routineRest() {
        resting = true;
        yield return 34f / 60f;
        resting = false;
    }
}