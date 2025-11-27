using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {
    public readonly Vector2 anchor, rope;
    public readonly string vinePath;
    private readonly float radius, maximumAngle, swingSpeed, acceleration;
    private readonly bool coyoteJump;
    private bool left = false;
    private float phase = 0f;
    private float maxAng = 0f;
    private float previousX, inputNeutralTime;

    public SwingSolid(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        anchor = data.Nodes[0] + levelOffset + Vector2.One * 24;
        rope = (Center - anchor).Length() * Vector2.UnitY;
        coyoteJump = data.Bool("coyoteJump");
        maximumAngle = data.Float("maximumAngle", 60f);
        swingSpeed = data.Float("swingSpeed", 1f);
        acceleration = data.Float("acceleration", 1f);
        Add(new Image(GFX.Game[data.Attr("sprite", "objects/GameHelper/swing/swing_solid")]) {
            RenderPosition = Vector2.One * -5
        });
        vinePath = data.Attr("chainSpritePrefix", "objects/GameHelper/swing/chain0");

        OnDashCollide = OnDash;
        radius = rope.Length();
        previousX = X;
        Depth = -2;
    }

    private DashCollisionResults OnDash(Player player, Vector2 direction) {
        if (direction.X != 0f) {
            Audio.Play("event:/GameHelper/swingblock/hit");
            Celeste.Freeze(0.05f);
            left = direction.X < 0f;
            maxAng = (float) (maximumAngle / 180f * Math.PI);
            // brute force valid phase value
            float bestDist = float.MaxValue;
            for (float i = (float) Math.PI / -2f; i < Math.PI / 2f; i += 0.01f) {
                float localDist = Math.Abs(X - getToX(left, maxAng, i));
                if (localDist < bestDist) {
                    phase = i;
                    bestDist = localDist;
                }
            }
            if (coyoteJump) {
                player.jumpGraceTimer = 0.1f;
            }
            return DashCollisionResults.Rebound;
        }
        return DashCollisionResults.NormalCollision;
    }

    public override void Update() {
        base.Update();

        if (Input.Aim.Value.X == 0f) {
            inputNeutralTime += Engine.DeltaTime;
        } else {
            inputNeutralTime = 0f;
        }

        // first acceleration
        if (maxAng <= 0f && HasPlayerClimbing() && Math.Sign(Input.Aim.Value.X) != 0) {
            left = Input.Aim.Value.X < 0f;
            maxAng += Engine.DeltaTime / 3f;
            phase = 0;
        }
        if (maxAng > 0f) {
            phase += swingSpeed * 2f * Engine.DeltaTime;
            float moveX = getToX(left, maxAng, phase);
            MoveToX(moveX);
            MoveToY(getToY(maxAng, phase));

            bool accelerate = HasPlayerClimbing() && ((inputNeutralTime < 0.5f && Math.Abs(Math.Cos(Math.Sin(phase) - Math.PI / 2)) > 0.8f) || Math.Sign(Input.Aim.Value.X) == Math.Sign(moveX - previousX));
            if (maxAng > 0f && !accelerate) {
                maxAng -= acceleration * Engine.DeltaTime / 6f;
            } else if (maxAng < (float) (maximumAngle / 180f * Math.PI) && accelerate) {
                maxAng += acceleration * Engine.DeltaTime / 6f;
            }
            previousX = moveX;
        } else {
            previousX = X;
        }
    }

    private float getToX(bool left, float maxAng, float phase) {
        float move = (left ? -1 : 1) * (float) (radius * Math.Cos(maxAng * Math.Sin(phase) - Math.PI / 2));
        return anchor.X - Width / 2 + move;
    }

    private float getToY(float maxAng, float phase) {
        float move = (float) (-radius * Math.Sin(maxAng * Math.Sin(phase) - Math.PI / 2));
        return anchor.Y - Height / 2 + move;
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        Center = anchor + rope;
        SceneAs<Level>().Add(new SwingSolidVine(this));
    }
}