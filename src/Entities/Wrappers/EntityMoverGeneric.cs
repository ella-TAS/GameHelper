using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Formats.Tar;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityMover")]
public class EntityMoverGeneric : EntityMover {
    private readonly float moveTime, nodeWaitTime;
    private readonly string flag, setFlagOnEnd, returnType, moveSound, nodeSound;
    private readonly bool additiveMovement, naiveMovement, holdPositionOnWait, stopMoveSoundOnStop;
    private float move, previous, waitTimer;
    private int prevNode = 0, nextNode = 1;
    private bool movingBack, playMoveSound = true;
    private Vector2 offset, waitPosition;
    private EventInstance moveSoundEvent;

    public EntityMoverGeneric(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        moveTime = Calc.Max(data.Float("moveTime"), Engine.DeltaTime);
        flag = data.Attr("flag");
        setFlagOnEnd = data.Attr("setFlagOnEnd");
        returnType = data.Attr("returnType");
        additiveMovement = data.Bool("additiveMovement");
        naiveMovement = data.Bool("naiveMovement");
        nodeSound = data.Attr("nodeSound");
        holdPositionOnWait = data.Bool("holdPositionOnWait");

        nodeWaitTime = data.Float("nodeWaitTime");
        moveSound = data.Attr("moveSound");
        stopMoveSoundOnStop = data.Bool("stopMoveSoundOnStop");
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();

        if(!Utils.Util.GetFlag(flag, Scene, true) || p == null || waitTimer > 0f) {
            if(holdPositionOnWait) {
                moveTo(waitPosition);
            }
            if(stopMoveSoundOnStop && Audio.IsPlaying(moveSoundEvent)) {
                Audio.Stop(moveSoundEvent);
            }
            if(waitTimer > 0f) {
                waitTimer -= Engine.DeltaTime;
                if(waitTimer <= 0f) {
                    playMoveSound = true;
                }
            } else {
                waitTimer -= Engine.DeltaTime;
            }
            return;
        }

        if(playMoveSound) {
            moveSoundEvent = Audio.Play(moveSound);
            playMoveSound = false;
        }

        move = Calc.Approach(move, moveTime, Engine.DeltaTime);
        float ease = Utils.Util.EaseMode(move / moveTime, easeMode);
        if(additiveMovement) {
            moveTo(target.Position + (ease - previous) * (nodes[nextNode] - nodes[prevNode]));
            previous = ease;
        } else {
            moveTo(offset + nodes[prevNode] + ease * (nodes[nextNode] - nodes[prevNode]));
        }
        waitPosition = target.Position;

        if(move != moveTime) return;
        // arrived at the next node
        waitTimer = nodeWaitTime;
        Audio.Play(nodeSound);

        if(!movingBack && nextNode == lastNode) {
            //arrived at the last node
            if(setFlagOnEnd.Length > 0 && !returnType.StartsWith("Move")) {
                SceneAs<Level>().Session.SetFlag(setFlagOnEnd);
            }
            switch(returnType) {
                case "Remove":
                    target.RemoveSelf();
                    RemoveSelf();
                    break;
                case "Teleport":
                    prevNode = 0;
                    nextNode = 1;
                    break;
                case "Move_Start":
                    prevNode = nextNode;
                    nextNode = 0;
                    movingBack = true;
                    break;
                case "Move_Path":
                    prevNode = nextNode;
                    nextNode--;
                    movingBack = true;
                    break;
                case "Stop":
                    RemoveSelf();
                    break;
            }
        } else if(movingBack && nextNode == 0) {
            //arrived at the first node
            movingBack = false;
            prevNode = 0;
            nextNode = 1;
            if(setFlagOnEnd.Length > 0) {
                SceneAs<Level>().Session.SetFlag(setFlagOnEnd);
            }
        } else {
            prevNode = nextNode;
            nextNode += movingBack ? -1 : 1;
        }
        move = previous = 0;
    }

    private void moveTo(Vector2 pos) {
        if(!naiveMovement && target is Actor) {
            Actor a = target as Actor;
            a.MoveToX(pos.X);
            a.MoveToY(pos.Y);
        } else if(!naiveMovement && target is Platform) {
            Platform s = target as Platform;
            s.MoveTo(pos);
        } else {
            target.Position = pos;
        }
    }

    public override void Awake(Scene scene) {
        target = FindNearest(Position, onlyType);
        if(target == null) {
            ComplainEntityNotFound("Generic Entity Mover");
            return;
        } else if(debug) {
            Logger.Info("GameHelper", "Generic EntityMover found " + target.GetType().ToString());
        }
        offset = target.Position - Position;
        waitPosition = target.Position;
        base.Awake(scene);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(setFlagOnEnd.Length > 0) {
            SceneAs<Level>().Session.SetFlag(setFlagOnEnd, false);
        }
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        if(stopMoveSoundOnStop && Audio.IsPlaying(moveSoundEvent)) {
            Audio.Stop(moveSoundEvent);
        }
    }
}