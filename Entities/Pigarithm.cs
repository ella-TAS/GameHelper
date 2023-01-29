//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Pigarithm")]
public class Pigarithm : Solid {
    private Level level;
    private Sprite sprite;
    private int speed;
    private bool movingRight;
    private bool kill;
    private int restTimer;
    private string size;

    public Pigarithm(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speed = data.Int("speed");
        movingRight = data.Bool("startRight");
        kill = data.Bool("kill");
        restTimer = 0;
        size = data.Attr("sprite");
        sprite = GameHelperModule.getSpriteBank().Create(size);
        if(size == "pigarithm_big") {
            sprite.RenderPosition = new Vector2(-8, 0);
        }
        Add(sprite);
    }

    public override void Update() {
        //player kill check
        Player p = Scene.Tracker.GetEntity<Player>();
        if(p != null && kill) {
            if(p.CollideCheck(this, p.Position + Vector2.UnitX) || p.CollideCheck(this, p.Position - Vector2.UnitX)) {
                p.Die(Vector2.Normalize(p.Center - this.Center));
            }
        }

        //movement
        if(restTimer > 0) {
            restTimer--;
        } else {
            bool collided = MoveHCollideSolidsAndBounds(level, (movingRight ? 1 : -1) * speed * Engine.DeltaTime, thruDashBlocks: true);
            if(collided) {
                movingRight = !movingRight;
                if(size == "pigarithm_big") {
                    sprite.Play("spin");
                    restTimer = 33;
                } else {
                    StartShaking(0.4f);
                    restTimer = 24;
                }
            }
        }

        base.Update();
    }

    public override void Render() {
        //shake animation, remove when turnaround sprite is added
        Vector2 position = Position;
        Position += base.Shake;
        base.Render();
        Position = position;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        level = SceneAs<Level>();
    }
}