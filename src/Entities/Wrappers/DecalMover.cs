using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/DecalMover")]
public class DecalMover : Wrapper {
    private Decal decal;
    private int nextNode;
    private Vector2 homePos;
    private readonly Vector2[] nodes;
    private readonly string flag;
    private readonly float speed;
    private readonly bool loop;

    public DecalMover(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        speed = data.Float("speed");
        flag = data.Attr("flag");
        loop = data.Bool("loop");
        nodes = new Vector2[data.Nodes.Length];
        for(int i = 0; i < nodes.Length; i++) {
            nodes[i] = data.Nodes[i] + levelOffset;
        }
    }

    public override void Update() {
        base.Update();
        if(flag?.Length == 0 || SceneAs<Level>().Session.GetFlag(flag)) {
            Position = Calc.Approach(Position, nodes[nextNode], speed * Engine.DeltaTime);
            if(Position == nodes[nextNode]) {
                nextNode++;
                if(nextNode >= nodes.Length) {
                    if(loop) {
                        Position = homePos;
                        nextNode = 0;
                    } else {
                        decal.RemoveSelf();
                        RemoveSelf();
                    }
                }
            }
            decal.Position = Position;
        }
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        decal = FindNearest<Decal>(Position);
        if(decal == null) {
            ComplainEntityNotFound("Decal Mover");
            return;
        }
        homePos = Position = decal.Position;
    }
}