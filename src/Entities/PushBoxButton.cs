using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PushBoxButton")]
public class PushBoxButton : Entity {
    private readonly Sprite sprite;
    private bool inside, wasInside;
    private readonly string flag;
    private readonly bool resetFlagOnDeath;

    public PushBoxButton(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        flag = data.Attr("flag");
        resetFlagOnDeath = data.Bool("resetFlagOnDeath");
        Add(sprite = GameHelper.SpriteBank.Create("push_box_button"));
        base.Collider = new Hitbox(16f, 8f);
        if(data.Bool("playerActivates")) {
            Add(new PlayerCollider(onCollide));
        }
        base.Depth = -2;
    }

    private void onCollide(Player player) {
        inside = true;
    }

    public override void Update() {
        base.Update();
        //push box collision
        if(!inside && CollideCheck<PushBox>()) {
            inside = true;
        }
        if(inside && !wasInside) {
            sprite.Play("down");
            SceneAs<Level>().Session.SetFlag(flag, true);
            wasInside = true;
        } else if(!inside && wasInside) {
            sprite.Play("idle");
            SceneAs<Level>().Session.SetFlag(flag, false);
            wasInside = false;
        }
        inside = false;
    }

    public override void Removed(Scene scene) {
        if(resetFlagOnDeath) {
            SceneAs<Level>().Session.SetFlag(flag, false);
        }
        base.Removed(scene);
    }
}