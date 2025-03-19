using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/JumpRope")]
public class JumpRope : Entity {
    private readonly List<RopeSegment> segments = new();
    private readonly Vector2 endVector;

    public JumpRope(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        Depth = 1;
        endVector = data.Nodes[0] + levelOffset - Position;
        if(endVector.X < 0) {
            // endVector is always positive
            Position = endVector;
            endVector *= -1f;
        }
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        float endX = X + endVector.X;
        if(p != null && X < p.X && p.X < endX && segments.Any(s => s.HasPlayerRider())) {
            segments.ForEach(s => s.BendByPlayer(1f, Calc.Clamp(p.X, X + 16f, endX - 16f)));
            if(p.StateMachine.State == PlayerState.StDash && p.DashDir.Y > 0.7f) {
                // avoid corner correction
                p.dashStartedOnGround = true;
                p.LiftSpeedGraceTime = 0.2f;
                p.LiftSpeed = new Vector2(0, -130);
                if(Input.Jump.Pressed) {
                    p.StateMachine.State = PlayerState.StNormal;
                    p.RefillDash();
                    // uncrouch 1f later
                    Add(Coroutines.Timeout(delegate {
                        p.Ducking = false;
                    }));
                }
            }
            if(p.LiftSpeed.Y > -100f) {
                p.LiftSpeedGraceTime = 0.05f;
                p.LiftSpeed = new Vector2(0, -60);
            }
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        Vector2 direction = endVector / endVector.X;
        float end = Position.X + endVector.X;

        // create a segment for every pixel until the end position
        for(Vector2 current = Position; current.X <= end; current += direction) {
            RopeSegment newSegment = new(current, X, end);
            SceneAs<Level>().Add(newSegment);
            segments.Add(newSegment);
        }
    }
}