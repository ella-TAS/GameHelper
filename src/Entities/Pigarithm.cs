using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Pigarithm")]
public class Pigarithm : Solid {
    private Level level;
    private Sprite sprite;
    private float speed;
    private bool movingRight, kill, resting;
    private string size, flag;

    public Pigarithm(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speed = data.Float("speed");
        movingRight = data.Bool("startRight");
        kill = data.Bool("kill");
        size = data.Attr("sprite");
        flag = data.Attr("flag");
        base.Depth = -1;
        sprite = GameHelper.SpriteBank.Create(size);
        sprite.RenderPosition = new Vector2(-8, 0);
        sprite.FlipY = data.Bool("flipSprite");
        Add(sprite);
    }

    public override void Update() {
        base.Update();

        //movement
        if(!resting && (flag == "" || SceneAs<Level>().Session.GetFlag(flag))) {
            bool collided = MoveHCollideSolidsAndBounds(level, (movingRight ? 1 : -1) * speed * Engine.DeltaTime, thruDashBlocks: true);
            if(collided) {
                movingRight = !movingRight;
                sprite.Play("spin");
                Add(new Coroutine(routineRest()));
            }
        }

        //player kill check
        Player p = Scene.Tracker.GetEntity<Player>();
        if(p != null && kill && (p.CollideCheck(this, p.Position + Vector2.UnitX) || p.CollideCheck(this, p.Position - Vector2.UnitX))) {
            p.Die((p.Center - this.Center).SafeNormalize());
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