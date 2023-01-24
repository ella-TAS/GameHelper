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
    private int restTimer;

    public Pigarithm(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speed = data.Int("speed");
        movingRight = data.Bool("startRight");
        restTimer = 0;
        Add(sprite = GameHelperModule.getSpriteBank().Create(data.Attr("sprite")));
    }

    public override void Update() {
        if(restTimer > 0) {
            restTimer--;
        } else {
            bool collided = MoveHCollideSolidsAndBounds(level, (movingRight ? 1 : -1) * speed * Engine.DeltaTime, thruDashBlocks: true);
            if(collided) {
                movingRight = !movingRight;
                StartShaking(0.4f);
                restTimer = 24;
            }
        }
        base.Update();
    }

    public override void Render() {
		Vector2 position = Position;
		Position += base.Shake;
		Draw.Rect(base.X + 2f, base.Y + 2f, base.Width - 4f, base.Height - 4f, Calc.HexToColor("8A9C60"));
		base.Render();
		Position = position;
	}

    public override void Added(Scene scene) {
        base.Added(scene);
        level = SceneAs<Level>();
    }
}