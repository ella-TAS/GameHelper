using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Utils;

[Tracked]
public class RopeSegment : JumpThru {
    private const int CORRECTION_ABOVE = 3;
    private const int CORRECTION_BELOW = 6;
    private const int MAX_PLAYER_BEND = 10;

    private readonly float anchorY, startX, endX;

    public RopeSegment(Vector2 position, Image sprite = null, float startX = 0f, float endX = 0f) : base(position.Round(), width: 1, safe: false) {
        Depth = -1;
        BlockWaterfalls = false;
        anchorY = position.Y;
        if (sprite != null) {
            sprite.RenderPosition = -Vector2.UnitY;
            Add(sprite);
        }
        this.startX = startX;
        this.endX = endX;
    }

    public void BendByPlayer(float factor, float playerX) {
        float endPoint = X < playerX ? startX : endX;
        float linearFactor = (X - endPoint) / (playerX - endPoint);
        MoveToY((float) Math.Round(anchorY + factor * linearFactor * MAX_PLAYER_BEND));
    }

    public override void DebugRender(Camera camera) {
        Draw.Point(Position, Color.Orange);
        Draw.Point(Position + Height * Vector2.UnitY, Color.Cyan);
    }

    public override void MoveVExact(int move) {
        // don't affect player liftboost
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        Vector2 liftBoost = default;
        float liftGrace = 0f;
        if (player != null) {
            liftBoost = player.LiftSpeed;
            liftGrace = player.LiftSpeedGraceTime;
        }

        base.MoveVExact(move);

        if (player != null) {
            player.LiftSpeed = liftBoost;
            player.LiftSpeedGraceTime = liftGrace;
        }
    }

    public static void Hook() {
        On.Celeste.Player.Update += PlayerUpdate;
    }

    public static void Unhook() {
        On.Celeste.Player.Update -= PlayerUpdate;
    }

    private static void PlayerUpdate(On.Celeste.Player.orig_Update orig, Player p) {
        orig(p);

        // reset V subpixels on the rope
        if (p.CollideCheck<RopeSegment>(p.Position + Vector2.UnitY)) {
            p.movementCounter.Y = 0f;
        }

        // avoid down-dash corner correction
        if (p.StateMachine.State == Player.StDash && p.DashDir.Y > 0 && p.CollideCheck<RopeSegment>(p.Position + new Vector2(0, 6))) {
            p.dashStartedOnGround = true;
        }

        if (p.Speed.Y <= -166f || p.CollideCheck<RopeSegment>(p.Position + new Vector2(0, -CORRECTION_BELOW))) {
            // dashing/boosting up or too far below
            return;
        }

        List<Entity> bridgeInside = p.CollideAll<RopeSegment>();
        if (bridgeInside.Count > 0) {
            float niveau = bridgeInside.MinBy(e => e.Position.Y).Y;
            p.MoveVExact((int) (niveau - p.Y));
            p.Speed.Y = Calc.Max(p.Speed.Y, -30f);
            p.varJumpTimer = 0f;
            p.movementCounter.Y = 0f;
        } else if (p.StateMachine.State == Player.StNormal && p.Speed.Y >= 0) {
            bridgeInside = p.CollideAll<RopeSegment>(p.Position + new Vector2(0, CORRECTION_ABOVE));
            if (bridgeInside.Count > 0) {
                float niveau = bridgeInside.MinBy(e => e.Position.Y).Y;
                p.MoveVExact((int) (niveau - p.Y));
                p.Speed.Y = 0;
            }
        }
    }
}