using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityMoverFollow")]
public class EntityMoverFollow : EntityMover {
    private readonly bool onlyX, onlyY, stopSoundOnLeave, holdPositionOnWait, awaitPlayerMovement;
    private readonly string flag, approachMode, approachSound, targetOnlyType;
    private readonly float speedFactor, playSoundAtDistance, minDistance, maxDistance;
    private readonly Vector2 offset;
    private Entity moveTarget;
    private Vector2 waitPosition;
    private EventInstance approachSoundEvent;
    private bool soundPlayed;

    public EntityMoverFollow(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        approachMode = data.Attr("approachMode");
        speedFactor = data.Float("speedFactor");
        flag = data.Attr("flag");
        onlyX = data.Bool("onlyX");
        onlyY = data.Bool("onlyY");
        targetOnlyType = data.Attr("targetOnlyType");
        approachSound = data.Attr("approachSound");
        playSoundAtDistance = data.Float("playSoundAtDistance");
        stopSoundOnLeave = data.Bool("stopSoundOnLeave");
        holdPositionOnWait = data.Bool("holdPositionOnWait");
        awaitPlayerMovement = data.Bool("awaitPlayerMovement");
        minDistance = data.Float("minDistance");
        maxDistance = data.Float("maxDistance");
        offset = new(data.Float("offsetX"), data.Float("offsetY"));
    }

    public override void Update() {
        base.Update();

        float dist = (moveTarget.Position + offset - target.Position).Length();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(
            (maxDistance > 0 && dist > maxDistance) ||
            dist <= minDistance ||
            !Utils.Util.GetFlag(flag, Scene, true) ||
            (p != null && awaitPlayerMovement && p.JustRespawned)
        ) {
            soundPlayed = false;
            if(holdPositionOnWait) {
                moveTo(waitPosition);
            }
            if(stopSoundOnLeave && dist > minDistance && Audio.IsPlaying(approachSoundEvent)) {
                Audio.Stop(approachSoundEvent);
            }
            return;
        }

        Vector2 targetPos = Calc.Approach(
            target.Position,
            moveTarget.Position + offset,
            approach(dist, approachMode, speedFactor)
        );
        if(onlyX ^ onlyY) {
            if(onlyX) {
                moveTo(new Vector2(targetPos.X, target.Position.Y));
            } else {
                moveTo(new Vector2(target.Position.X, targetPos.Y));
            }
        } else {
            moveTo(targetPos);
        }
        waitPosition = target.Position;

        if(approachSound.Length == 0) return;

        if((moveTarget.Position + offset - target.Position).Length() <= playSoundAtDistance) {
            if(!soundPlayed) {
                soundPlayed = true;
                approachSoundEvent = Audio.Play(approachSound);
            }
        } else {
            soundPlayed = false;
            if(Audio.IsPlaying(approachSoundEvent)) {
                Audio.Stop(approachSoundEvent);
            }
        }
    }

    private static float approach(float dist, string modus, float factor) {
        if(modus.StartsWith("Hyperbolic") || modus.StartsWith("Inverse")) {
            dist = Math.Max(dist, 1f);
        }
        dist /= 60f;
        return modus switch {
            "Exponential" => (float) Math.Pow(2, dist * factor),
            "Cubic" => factor * dist * dist * dist,
            "Quadratic" => factor * dist * dist,
            "Linear" => factor * dist,
            "Constant" => factor,
            "Hyperbolic" => factor / dist,
            "Inverse Quadratic" => factor / dist / dist,
            "Inverse Cubic" => factor / dist / dist / dist,
            "Inverse Exponential" => 1 / (float) Math.Pow(2, dist / factor),
            _ => 0,
        };
    }

    public override void Awake(Scene scene) {
        target = FindNearest(nodes[0], onlyType);
        moveTarget = FindNearest(nodes[1], targetOnlyType, target);
        if(target == null || moveTarget == null) {
            Logger.Warn("GameHelper",
                "Follow Entity Mover found " +
                (target == null ? "no entity" : "entity " + target.GetType().ToString()) + ", " +
                (moveTarget == null ? "no target" : "target " + moveTarget.GetType().ToString()) +
                " in room " + SceneAs<Level>().Session.LevelData.Name
            );
            RemoveSelf();
            return;
        }
        if(debug) {
            Logger.Info("GameHelper", "Follow Entity Mover found entity " + target.GetType().ToString() + " and target " + moveTarget.GetType().ToString());
        }
        waitPosition = target.Position;
        base.Awake(scene);
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        if(stopSoundOnLeave && Audio.IsPlaying(approachSoundEvent)) {
            Audio.Stop(approachSoundEvent);
        }
    }
}