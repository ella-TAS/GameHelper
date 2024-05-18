using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/McFire")]
public class McFire : Entity {
    private const float PI = 3.14159274101257324219f;
    private readonly float delay;
    private readonly Sprite sprite;
    private readonly int rotation;

    public McFire(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        delay = data.Float("spreadingTime");
        sprite = GameHelper.SpriteBank.Create("fire");
        sprite.RenderPosition = new Vector2(-8, -8);
        Add(sprite);
        Collider = new Hitbox(16, 16, -8, -8);
        rotation = int.Parse(data.Attr("direction"));
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        sprite.Rotation = 0.5f * PI * rotation;
    }
}

[CustomEntity("GameHelper/McFlammable")]
public class McFlammable : Solid {
    public McFlammable(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {

    }
}