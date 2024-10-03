using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PushBox")]
[Tracked]
public class PushBox : Solid {
    private const float gravity = 7.5f;
    private const float fallCap = 160f;
    private readonly Color colorBorder = Calc.HexToColor("e58125");
    private readonly Color colorFill = Calc.HexToColor("fbb954");
    private readonly Color colorCorner = Calc.HexToColor("8ff8e2");
    private readonly float speedX;
    private float velY;

    public PushBox(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        speedX = data.Float("speedX");
        Add(GameHelper.SpriteBank.Create("pigarithm_small"));
        Depth = -1;
    }

    public override void Update() {
        base.Update();

        //player check, move X
        Player p = Scene.Tracker.GetEntity<Player>();
        if(p != null && !HasPlayerClimbing()) {
            if(p.CollideCheck(this, p.Position + Vector2.UnitX)) { //moving right
                MoveHor(speedX * Engine.DeltaTime);
            } else if(p.CollideCheck(this, p.Position - Vector2.UnitX)) { //moving left
                MoveHor(-speedX * Engine.DeltaTime);
            }
        }

        //move Y
        velY = Calc.Approach(velY, fallCap, gravity);
        if(MoveVCollideSolids(velY * Engine.DeltaTime, thruDashBlocks: true)) {
            velY = 0f;
        }
        if(Top > SceneAs<Level>().Bounds.Bottom + 32f) {
            RemoveSelf();
        }
    }

    public void MoveHor(float speedDt) {
        MoveHCollideSolids(speedDt, thruDashBlocks: true);
    }

    public void MoveVer(float speedDt) {
        MoveVCollideSolids(speedDt, thruDashBlocks: true);
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

    public static void Hook() {
        On.Celeste.Solid.MoveHExact += OnSolidMoveHExact;
        On.Celeste.Solid.MoveVExact += OnSolidMoveVExact;
    }

    public static void Unhook() {
        On.Celeste.Solid.MoveHExact -= OnSolidMoveHExact;
        On.Celeste.Solid.MoveVExact -= OnSolidMoveVExact;
    }

    //move boxes above any platform
    private static void OnSolidMoveHExact(On.Celeste.Solid.orig_MoveHExact orig, Solid self, int movedPx) {
        foreach(PushBox box in self.CollideAll<PushBox>(self.Position - 3 * Vector2.UnitY)) {
            if(box.Bottom > self.Position.Y) {
                continue;
            }
            box.MoveHor(movedPx);
        }
        orig(self, movedPx);
    }

    private static void OnSolidMoveVExact(On.Celeste.Solid.orig_MoveVExact orig, Solid self, int movedPx) {
        foreach(PushBox box in self.CollideAll<PushBox>(self.Position - 3 * Vector2.UnitY)) {
            if(box.Bottom > self.Position.Y) {
                continue;
            }
            if(movedPx > 0) {
                box.MoveV(movedPx);
            }
            box.MoveVer(movedPx);
        }
        orig(self, movedPx);
    }
}