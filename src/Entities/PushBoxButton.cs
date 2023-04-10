using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PushBoxButton")]
public class PushBoxButton : Entity {
    private Sprite sprite;
    private int hasCollided, collidable;
    private bool down, wasDown;
    private string flag;
    private bool playerActivates, resetFlagOnDeath;

    public PushBoxButton(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        flag = data.Attr("flag");
        playerActivates = data.Bool("playerActivates");
        resetFlagOnDeath = data.Bool("resetFlagOnDeath");
        hasCollided = collidable = 0;
        down = wasDown = false;
        Add(sprite = GameHelper.SpriteBank.Create("push_box_button"));
        base.Collider = new Hitbox(16f, 8f);
        if(playerActivates) {
            Add(new PlayerCollider(onCollide));
        }
        base.Depth = -2;
    }

    private void onCollide(Player player) {
        down = true;
        collidable++;
    }

    public override void Update() {
        base.Update();
        down = false;
        if(collidable != 0) {
            hasCollided++;
            if(collidable == hasCollided) {
                down = true;
            } else {
                collidable = hasCollided = 0;
            }
        }
        if(!down) {
            foreach(Entity e in Scene.Tracker.GetEntities<PushBox>()) {
                if(CollideCheck(e)) {
                    down = true;
                    break;
                }
            }
        }
        if(down && !wasDown) {
            sprite.Play("down");
            wasDown = true;
            SceneAs<Level>().Session.SetFlag(flag, true);
        } else if(!down && wasDown) {
            sprite.Play("idle");
            wasDown = false;
            SceneAs<Level>().Session.SetFlag(flag, false);
        }
    }

    public override void Removed(Scene scene) {
        if(resetFlagOnDeath) {
            SceneAs<Level>().Session.SetFlag(flag, false);
        }
        base.Removed(scene);
    }
}