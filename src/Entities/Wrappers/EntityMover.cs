using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;


public class EntityMover : Wrapper {
    protected Entity target;
    protected readonly string onlyType, easeMode;
    protected readonly int lastNode;
    protected readonly bool debug;
    protected readonly Vector2[] nodes;

    public EntityMover(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        onlyType = data.Attr("onlyType");
        easeMode = data.Attr("easeMode");
        debug = data.Bool("debug");
        lastNode = data.Nodes.Length; // node 0 is home Position
        nodes = new Vector2[lastNode + 1];
        for(int i = 0; i < lastNode; i++) {
            nodes[i + 1] = data.Nodes[i] + levelOffset;
        }
        nodes[0] = Position;
        Depth = 999999999;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(debug) {
            LogAllEntities();
        }
    }
}