using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

[CustomEntity("GameHelper/MouseHole")]
public class MouseHole : Entity {
    private Sprite sprite;
    private bool wasFlag, complete;
    private float spawnTimer;
    private bool spawner;
    private string flag;

    public MouseHole(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        spawner = data.Bool("spawner");
        flag = data.Attr("flag");
        base.Depth = 2;
        base.Collider = new Hitbox(16, 16);
        Add(sprite = GameHelperModule.SpriteBank.Create("mouse_hole"));
        if(!spawner) {
            sprite.Play("exit");
        }
    }

    public override void Update() {
        base.Update();
        if(!spawner) {
            foreach(Mouse m in CollideAll<Mouse>()) {
                m.RemoveSelf();
                if(!complete) {
                    SceneAs<Level>().Session.SetFlag(flag, true);
                    sprite.Play("complete");
                    complete = true;
                }
            }
        } else {
            spawnTimer += Engine.DeltaTime;
            bool isFlag = SceneAs<Level>().Session.GetFlag(flag);
            if(isFlag && !wasFlag) {
                sprite.Play("opening");
                wasFlag = true;
                spawnTimer = 0.2f;
            }
            if(!isFlag && wasFlag) {
                sprite.Play("closing");
                wasFlag = false;
            }
            if(isFlag && spawnTimer >= 1f/3f) {
                SceneAs<Level>().Add(new Mouse(Position + new Vector2(5, 6)));
                spawnTimer = 0;
            }
        }
    }
}