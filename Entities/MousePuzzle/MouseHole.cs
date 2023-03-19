using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

[CustomEntity("GameHelper/MouseHole")]
public class MouseHole : Entity {
    private bool spawner;
    private string flag;

    public MouseHole(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        spawner = data.Bool("spawner");
        flag = data.Attr("flag");
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();

    }
}