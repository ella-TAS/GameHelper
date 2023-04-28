using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Wrappers;

[CustomEntity("GameHelper/DecalMover")]
public class DecalMover : Entity {
    private Decal decal;
    private int nextNode;
    private Vector2 homePos;
    private Vector2[] nodes;
    private string flag;
    private float speed;
    private bool loop;

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
        if(flag == "" || SceneAs<Level>().Session.GetFlag(flag)) {
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
        float minDistance = float.MaxValue;
        foreach(Decal d in scene.Entities.FindAll<Decal>()) {
            if(decal == null || Vector2.Distance(d.Position, Position) < minDistance) {
                decal = d;
                minDistance = Vector2.Distance(d.Position, Position);
            }
        }
        if(decal == null) {
            Logger.Log(LogLevel.Warn, "GameHelper", "Decal Mover found no decal in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
        homePos = Position = decal.Position;
    }
}