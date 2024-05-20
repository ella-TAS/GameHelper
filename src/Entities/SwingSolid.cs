using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {
    private const float PI = 3.1415927f;
    private readonly Vector2 anchor, rope;
    private readonly float maxAngle, swingSpeed, accelTime;
    private readonly int reverse;
    private float wiggleTimer, strength;

    public SwingSolid(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        anchor = data.Nodes[0] + levelOffset + Vector2.One * 24;
        rope = (Center - anchor).Length() * Vector2.UnitY;
        Add(new Image(GFX.Game[data.Attr("sprite", "objects/GameHelper/swing/swing_solid")]) {
            RenderPosition = Vector2.One * -5
        });
        maxAngle = Math.Abs(data.Float("maxAngle", 30f));
        swingSpeed = Math.Abs(data.Float("swingSpeed", 1f));
        accelTime = Math.Max(data.Float("accelerationTime", 377f), 1f);
        reverse = data.Bool("startRight") ? -1 : 1;
    }

    public override void Update() {
        base.Update();
        strength = Calc.Approach(strength, HasPlayerRider() ? 1f : 0f, 1 / accelTime);
        wiggleTimer += Engine.DeltaTime;
        if(strength == 0f) wiggleTimer = 0f;
        Vector2 newCenter = anchor + rope.Rotate(maxAngle / 180f * PI * (float) Math.Sin(wiggleTimer * swingSpeed * reverse) * strength);
        MoveH((float) Math.Round(newCenter.X - Center.X));
        MoveV((float) Math.Round(newCenter.Y - Center.Y));
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        Center = anchor + rope;
    }
}