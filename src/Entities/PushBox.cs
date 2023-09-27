using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PushBox")]
[Tracked]
public class PushBox : Solid {
    private const float gravity = 7.5f;
    private const float fallCap = 160f;
    private Color colorBorder = Calc.HexToColor("e58125");
    private Color colorFill = Calc.HexToColor("fbb954");
    private Color colorCorner = Calc.HexToColor("8ff8e2");
    private readonly float speedX;
    private float velY;

    public PushBox(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speedX = data.Float("speedX");
        Add(GameHelper.SpriteBank.Create("pigarithm_small"));
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();

        //player check, move X
        Player p = Scene.Tracker.GetEntity<Player>();
        if(p != null && !HasPlayerClimbing()) {
            if(p.CollideCheck(this, p.Position + Vector2.UnitX)) {
                MoveHCollideSolids(speedX * Engine.DeltaTime, thruDashBlocks: true);
            } else if(p.CollideCheck(this, p.Position - Vector2.UnitX)) {
                MoveHCollideSolids(-speedX * Engine.DeltaTime, thruDashBlocks: true);
            }
        }

        //move Y
        velY = Calc.Approach(velY, fallCap, gravity);
        if(MoveVCollideSolids(velY * Engine.DeltaTime, thruDashBlocks: true)) {
            velY = 0f;
        }
        if(base.Top > SceneAs<Level>().Bounds.Bottom + 32f) {
            RemoveSelf();
        }
    }

    public override void Render() {
        Vector2 w = Vector2.UnitX * (Width - 4);
        Vector2 h = Vector2.UnitY * (Height - 4);
        Vector2 p = Position + (Vector2.One * 2);
        Draw.Rect(Position.X, Position.Y, Width, Height, Color.Black);
        Draw.Rect(Position.X + 1, Position.Y + 1, Width - 2, Height - 2, colorFill);
        Draw.Line(p, p + w, colorBorder, 2);
        Draw.Line(p + h, p + h + w, colorBorder, 2);
        Draw.Line(p, p + h, colorBorder, 2);
        Draw.Line(p + w, p + h + w, colorBorder, 2);
        Draw.Line(p, p + h + w, colorBorder, 2);
        Draw.Rect(Position.X + 1, Position.Y + 1, 2, 2, colorCorner);
        Draw.Rect(Position.X + Width - 3, Position.Y + 1, 2, 2, colorCorner);
        Draw.Rect(Position.X + 1, Position.Y + Height - 3, 2, 2, colorCorner);
        Draw.Rect(Position.X + Width - 3, Position.Y + Height - 3, 2, 2, colorCorner);
    }
}