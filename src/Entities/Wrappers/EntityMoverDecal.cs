using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.GameHelper.Utils;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/DecalMover")]
public class EntityMoverDecal : EntityMover {
    private int nextNode;
    private bool movingBack;
    private readonly string flag;
    private readonly float speed;
    private readonly byte returnType; // Remove = 0, Teleport = 1, Move_Start = 2, Move_Path = 3, Stop = 4
    private readonly bool flipX, flipY;

    public EntityMoverDecal(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        speed = data.Float("speed");
        flag = data.Attr("flag");
        returnType = (byte) data.Int("returnType", data.Bool("loop", true) ? 1 : 0); //loop for legacy placements
        flipX = data.Bool("flipX");
        flipY = data.Bool("flipY");
        nextNode = 1;
    }

    public override void Update() {
        base.Update();
        if(!Util.GetFlag(flag, Scene, true)) {
            return;
        }
        target.Position = Position = Calc.Approach(Position, nodes[nextNode], speed * Engine.DeltaTime);
        if(Position != nodes[nextNode]) {
            return;
        }
        //arrived at next node
        if(!movingBack && nextNode == lastNode) {
            //arrived at the end node
            switch(returnType) {
                case 0:
                    target.RemoveSelf();
                    RemoveSelf();
                    break;
                case 1:
                    Position = nodes[0];
                    nextNode = 1;
                    break;
                case 2:
                    nextNode = 0;
                    movingBack = true;
                    break;
                case 3:
                    nextNode--;
                    movingBack = true;
                    break;
                case 4:
                    RemoveSelf();
                    break;
            }
            if(returnType != 0) {
                Decal decal = target as Decal;
                decal.Scale = flip(decal.Scale);
            }
        } else if(movingBack && nextNode == 0) {
            //arrived at the start node
            movingBack = false;
            nextNode = 1;
            Decal decal = target as Decal;
            decal.Scale = flip(decal.Scale);
        } else {
            nextNode += movingBack ? -1 : 1;
        }
    }

    private Vector2 flip(Vector2 vector) {
        if(flipX) {
            vector.X *= -1;
        }
        if(flipY) {
            vector.Y *= -1;
        }
        return vector;
    }

    public override void Awake(Scene scene) {
        target = FindNearest<Decal>(Position);
        if(target == null) {
            ComplainEntityNotFound("Decal Entity Mover");
            return;
        }
        base.Awake(scene);
    }
}