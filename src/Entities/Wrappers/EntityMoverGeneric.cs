using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System.Formats.Tar;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/EntityMover")]
public class EntityMoverGeneric : EntityMover {
    private readonly float moveTime;
    private readonly string flag, setFlagOnEnd, returnType;
    private readonly bool additiveMovement, naiveMovement;
    private float move, previous;
    private int prevNode = 0, nextNode = 1;
    private bool movingBack;
    private Vector2 offset;

    public EntityMoverGeneric(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        moveTime = data.Float("moveTime");
        flag = data.Attr("flag");
        setFlagOnEnd = data.Attr("setFlagOnEnd");
        returnType = data.Attr("returnType");
        additiveMovement = data.Bool("additiveMovement");
        naiveMovement = data.Bool("naiveMovement");
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(!Util.GetFlag(flag, Scene, true) || p == null) {
            return;
        }

        move = Calc.Approach(move, moveTime, Engine.DeltaTime);
        float ease = Util.EaseMode(move / moveTime, easeMode);
        if(additiveMovement) {
            moveTo(target.Position + (ease - previous) * (nodes[nextNode] - nodes[prevNode]));
            previous = ease;
        } else {
            moveTo(offset + nodes[prevNode] + ease * (nodes[nextNode] - nodes[prevNode]));
        }

        if(move != moveTime) return;
        // arrived at the next node

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
        base.Awake(scene);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(setFlagOnEnd.Length > 0) {
            SceneAs<Level>().Session.SetFlag(setFlagOnEnd, false);
        }
    }
}