using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Pigarithm")]
public class Pigarithm : Solid {
    private const float gravity = 7.5f;
    private const float fallCap = 160f;
    private Level level;
    private readonly Sprite sprite;
    private readonly float speedX;
    private float speedY;
    private bool movingRight, resting;
    private readonly bool kill, hasGravity, mole;
    private readonly string size, flag;

    public Pigarithm(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speedX = data.Float("speed");
        movingRight = data.Bool("startRight");
        kill = data.Bool("kill");
        size = data.Attr("sprite");
        flag = data.Attr("flag");
        hasGravity = data.Bool("gravity");
        mole = size == "pigarithm_mole";
        base.Depth = -1;
        sprite = GameHelper.SpriteBank.Create(size);
        if(!mole) {
            sprite.RenderPosition = new Vector2(-8, 0);
        }
        sprite.FlipY = data.Bool("flipSprite");
        sprite.FlipX = mole && !movingRight;
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
        if(!resting && (flag?.Length == 0 || SceneAs<Level>().Session.GetFlag(flag))) {
            bool collided = MoveHCollideSolidsAndBounds(level, (movingRight ? 1 : -1) * speedX * Engine.DeltaTime, thruDashBlocks: true);
            if(!collided) {
                foreach(SeekerBarrier s in SceneAs<Level>().Tracker.GetEntities<SeekerBarrier>()) {
                    if(s.CollideCheck(this)) {
                        collided = true;
                        break;
                    }
                }
            }
            if(collided) {
                movingRight = !movingRight;
                if(mole) {
                    sprite.FlipX = !sprite.FlipX;
                } else {
                    sprite.Play("spin");
                    Add(new Coroutine(routineRest()));
                }
            }
        }

        //y movement
        if(hasGravity) {
            speedY = Calc.Approach(speedY, fallCap, gravity);
            if(MoveVCollideSolids(speedY * Engine.DeltaTime, thruDashBlocks: true)) {
                speedY = 0f;
            }
            if(base.Top > (float) SceneAs<Level>().Bounds.Bottom + 8f) {
                RemoveSelf();
            }
        }
    }

    private IEnumerator routineRest() {
        resting = true;
        yield return 34f / 60f;
        resting = false;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        level = SceneAs<Level>();
    }
}