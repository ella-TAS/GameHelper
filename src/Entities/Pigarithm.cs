using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

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
        Depth = 1;
        sprite = GameHelper.SpriteBank.Create(data.Attr("sprite"));
        sprite.RenderPosition = new Vector2(-8, 0);
        sprite.FlipY = data.Bool("flipSprite");
        Add(sprite);
    }

    public override void Update() {
        if(!resting && Util.GetFlag(flag, Scene, true)) {
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

        base.Update();
    }

    private IEnumerator routineRest() {
        resting = true;
        yield return 34f / 60f;
        resting = false;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(!kill) return;
        Spikes s;
        s = new(
            TopLeft + new Vector2(2, 0),
            (int) Height,
            Spikes.Directions.Left,
            "default"
        );
        s.Visible = false;
        SceneAs<Level>().Add(s);
        Add(new EntityMoveComponent(s));
        s = new(
            TopRight + new Vector2(-2, 0),
            (int) Height,
            Spikes.Directions.Right,
            "default"
        );
        s.Visible = false;
        SceneAs<Level>().Add(s);
        Add(new EntityMoveComponent(s));
    }
}