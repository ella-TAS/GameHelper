using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/DecalMover")]
public class DecalMover : Wrapper {
    private Decal decal;
    private int nextNode;
    private bool movingBack;
    private readonly int lastNode;
    private readonly Vector2[] nodes;
    private readonly string flag;
    private readonly float speed;
    private readonly byte returnType; //Remove = 0, Teleport = 1, Move_Start = 2, Move_Path = 3, Stop = 4
    private readonly bool flipX, flipY;

    public DecalMover(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        speed = data.Float("speed");
        flag = data.Attr("flag");
        returnType = (byte) data.Int("returnType", data.Bool("loop", true) ? 1 : 0); //loop for legacy placements
        flipX = data.Bool("flipX");
        flipY = data.Bool("flipY");
        lastNode = data.Nodes.Length; //node 0 is home Position
        nodes = new Vector2[lastNode + 1];
        for(int i = 0; i < lastNode; i++) {
            nodes[i + 1] = data.Nodes[i] + levelOffset;
        }
        nextNode = 1;
    }

    public override void Update() {
        base.Update();
        if(flag.Length != 0 && !SceneAs<Level>().Session.GetFlag(flag)) {
            return;
        }
        decal.Position = Position = Calc.Approach(Position, nodes[nextNode], speed * Engine.DeltaTime);
        if(Position != nodes[nextNode]) {
            return;
        }
        //arrived at next node
        if(!movingBack && nextNode == lastNode) {
            //arrived at the end node
            switch(returnType) {
                case 0:
                    decal.RemoveSelf();
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
                decal.SetScale(flip(decal.Scale));
            }
        } else if(movingBack && nextNode == 0) {
            //arrived at the start node
            movingBack = false;
            nextNode = 1;
            decal.SetScale(flip(decal.Scale));
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
        base.Awake(scene);
        decal = FindNearest<Decal>(Position);
        if(decal == null) {
            ComplainEntityNotFound("Decal Mover");
            return;
        }
        nodes[0] = Position = decal.Position;
    }
}