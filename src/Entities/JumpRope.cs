using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/JumpRope")]
public class JumpRope : Entity {
    private const float MOVE_TIME = 0.5f;
    private const float MOVE_BACK_TIME = 0.2f;

    private readonly List<RopeSegment> segments = new();
    private readonly Vector2 endVector;
    private readonly bool canBend;
    private readonly Image leftEnd, rightEnd;
    private readonly string texturePath;
    private float moveTimer, lastPlayerX;
    private bool wasPlayer, dashed;

    public JumpRope(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        Depth = -2;
        endVector = data.Nodes[0] + levelOffset - Position;
        if (endVector.X < 0) {
            // make endVector always positive
            Position = endVector;
            endVector *= -1f;
        }
        canBend = endVector.X >= 32f;

        texturePath = Util.TexturePathClean(data, "texturePath", "objects/GameHelper/rope/");

        if (data.Bool("renderLeftEnd")) {
            leftEnd = new(GFX.Game[texturePath + "jump_rope_blob"]) {
                Position = Position - new Vector2(4, 2)
            };
        }
        if (data.Bool("renderRightEnd")) {
            rightEnd = new(GFX.Game[texturePath + "jump_rope_blob"]) {
                Position = data.Nodes[0] + levelOffset - new Vector2(4, 2)
            };
        }
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        float endX = X + endVector.X;

        if (p != null && segments.Any(s => s.HasPlayerRider()) && canBend) {
            // bending
            lastPlayerX = Calc.Clamp(p.X, X + 16f, endX - 16f);
            moveTimer = Calc.Approach(moveTimer, MOVE_TIME, Engine.DeltaTime * (dashed ? 4 : 1));
            segments.ForEach(s => s.BendByPlayer(Ease.QuintOut(moveTimer / MOVE_TIME), lastPlayerX));

            // down dash behavior
            if (p.StateMachine.State == Player.StDash && p.DashDir.Y > 0.7f) {
                dashed = true;
                p.LiftSpeedGraceTime = 0.2f;
                p.LiftSpeed = new Vector2(0f, -130f);
                // roost is only allowed if the rope is already properly bent
                if (moveTimer / MOVE_TIME > 0.75f && Input.Jump.Pressed) {
                    p.StateMachine.State = Player.StNormal;
                    p.RefillDash();
                    // uncrouch 1f later
                    Add(Coroutines.Timeout(delegate {
                        p.Ducking = false;
                    }));
                }
            }

            // only give small liftboost if no bigger one exists
            float boost = Math.Min(2f * moveTimer / MOVE_TIME + 0.2f, 1f);
            if (p.LiftSpeed.Y >= -60f * boost) {
                p.LiftSpeedGraceTime = 0.05f;
                p.LiftSpeed = new Vector2(0f, -60f * boost);
            }

            wasPlayer = true;
        } else {
            if (wasPlayer) {
                float target = Ease.QuintOut(moveTimer / MOVE_TIME);
                float i;
                for (i = 0f; Ease.ExpoIn(i) < target; i += 0.01f) ;
                moveTimer = i * MOVE_BACK_TIME;
            }

            // unbending
            moveTimer = Calc.Approach(moveTimer, 0, Engine.DeltaTime);
            segments.ForEach(s => s.BendByPlayer(Ease.ExpoIn(moveTimer / MOVE_BACK_TIME), lastPlayerX));


            wasPlayer = false;
            dashed = false;
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        Vector2 direction = endVector / endVector.X;
        float end = Position.X + endVector.X;

        // create a segment for every pixel until the end position
        int counter = 0;
        for (Vector2 current = Position; current.X <= end; current += direction) {
            Image sprite = new(GFX.Game[texturePath + "jump_rope_" + (counter % 3)]);
            RopeSegment newSegment = new(current, sprite, X, end);
            SceneAs<Level>().Add(newSegment);
            segments.Add(newSegment);
            counter++;
        }
    }

    public override void Render() {
        base.Render();
        leftEnd?.Render();
        rightEnd?.Render();
    }
}