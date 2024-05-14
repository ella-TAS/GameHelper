using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/McFire")]
public class McFire : Entity {
    private readonly float delay;
    private readonly Sprite sprite;

    public McFire(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        delay = data.Float("spreadingTime");
        Add(sprite = GameHelper.SpriteBank.Create("fire"));
    }
}

[CustomEntity("GameHelper/McFlammable")]
public class McFlammable : Solid {
    public McFlammable(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {

    }
}