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
        base.Collider = new Hitbox(16, 16);
    }

    public override void Update() {
        base.Update();
        if(!spawner) {
            foreach(Mouse m in CollideAll<Mouse>()) {
                m.RemoveSelf();
                SceneAs<Level>().Session.SetFlag(flag);
            }
        } else if(SceneAs<Level>().Session.GetFlag(flag) && Scene.OnInterval(0.1f)) {
            Scene.Add(new Mouse(Collider.Center));
        }
    }
}