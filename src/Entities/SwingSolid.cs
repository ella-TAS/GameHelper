using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {
    private const float PI = 3.1415927f;
    private readonly Vector2 anchor, rope;
    private readonly bool stickOnDash;
    private readonly float radius;
    private readonly float constSwingAngle = 0; // (float) (Math.PI / 8f);

    private bool left = false;
    private float phase = 0f;
    private float maxAng = 0f;
    public SwingSolid(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        anchor = data.Nodes[0] + levelOffset + Vector2.One * 24;
        rope = (Center - anchor).Length() * Vector2.UnitY;
        Add(new Image(GFX.Game[data.Attr("sprite", "objects/GameHelper/swing/swing_solid")]) {
            RenderPosition = Vector2.One * -5
        });

        OnDashCollide = OnDash;
        stickOnDash = false; //stickOnDash;
        OnDashCollide = OnDash;
        radius = rope.Length();
    }


    private DashCollisionResults OnDash(Player player, Vector2 direction) {
        if(maxAng <= 0f && direction.X != 0f && constSwingAngle == 0f) {
            left = direction.X < 0f;
            maxAng = (float) (Math.PI / 3);
            phase = 0;
            if(!stickOnDash) return DashCollisionResults.Rebound;
        }
        return DashCollisionResults.NormalCollision;
    }

    public override void Update() {
        base.Update();

        if(maxAng <= 0f && HasPlayerClimbing() && Math.Sign(Input.Aim.value.X) != 0) {
            left = Input.Aim.value.X < 0f;
            maxAng += Engine.DeltaTime / 6f;
        }
        if(maxAng > 0f || constSwingAngle != 0f) {
            phase += 2 * Engine.DeltaTime;
            float moveToX = (left ? -1 : 1) * (float) (radius * Math.Cos((maxAng + constSwingAngle) * Math.Sin(phase) - Math.PI / 2));
            float moveToY = (float) (-radius * Math.Sin((maxAng + constSwingAngle) * Math.Sin(phase) - Math.PI / 2));
            MoveToX(anchor.X - Width / 2 + moveToX);
            MoveToY(anchor.Y - Height / 2 + moveToY);

            bool keepMomentum = HasPlayerClimbing() && Math.Sign(Input.Aim.Value.X) == Math.Sign(moveToX);
            if(maxAng > 0f && !keepMomentum)
                maxAng -= Engine.DeltaTime / 6f;
            else if(maxAng < Math.PI / 3f && keepMomentum) {
                maxAng += Engine.DeltaTime / 6f;
            }
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        Center = anchor + rope;
    }

    public override void Render() {
        base.Render();
        Draw.Line(anchor, Center, Color.White, 3f);
    }
}