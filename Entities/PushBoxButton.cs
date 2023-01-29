//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PushBoxButton")]
public class PushBoxButton : Entity {
    private Sprite sprite;

    public PushBoxButton(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        Add(sprite);
        base.Collider = new Hitbox(16f, 4f, 0f, 0f);
        Add(new PlayerCollider(onCollide));
    }

    private void onCollide(Player player) {

    }

    public override void Update() {
        base.Update();
    }
}