using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {
    private const float PI = 3.1415927f;
    private readonly Vector2 anchor, rope;
    private float wiggleTimer, strength;

    public SwingSolid(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        anchor = data.Nodes[0] + levelOffset + Vector2.One * 24;
        rope = (Center - anchor).Length() * Vector2.UnitY;
        Add(new Image(GFX.Game["objects/GameHelper/swing/swing_solid"]) {
            RenderPosition = Vector2.One * -5
        });
    }

    public override void Update() {
        base.Update();

        wiggleTimer += Engine.DeltaTime;
        Vector2 newCenter = anchor + rope.Rotate(PI / 6 * (float) Math.Sin(wiggleTimer));
        MoveH((float) Math.Round(newCenter.X - Center.X));
        MoveV((float) Math.Round(newCenter.Y - Center.Y));
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        Center = anchor + rope;
    }
}