using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/McFire")]
public class McFire : Entity {
    private const float PI = 3.14159274101257324219f;
    private readonly float delay;
    private readonly Sprite sprite;
    private readonly int preferredRotation;
    private McFlammable fuel;

    public McFire(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        delay = data.Float("spreadingTime");
        sprite = GameHelper.SpriteBank.Create("fire");
        sprite.RenderPosition = new Vector2(-8, -8);
        Add(sprite);
        Collider = new Hitbox(16, 16, -8, -8);
        preferredRotation = int.Parse(data.Attr("direction"));
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        int rotation = determineRotation();
        sprite.Rotation = rotation * 0.5f * PI;
        sprite.RenderPosition += new Vector2(0 < rotation && rotation < 3 ? 16 : 0, 1 < rotation ? 16 : 0);
    }

    private int determineRotation() {
        for(int i = 0; i < 4; i++) {
            Vector2 p = Position + Vector2.UnitY.Rotate((preferredRotation + i) * 0.5f * PI);
            if(CollideCheck<McFlammable>(p) {
                fuel = CollideFirst<McFlammable>(p);
                return (preferredRotation + i) % 4;
            }
        }
        for(int i = 0; i < 4; i++) {
            if(CollideCheck<Solid>(Position + Vector2.UnitY.Rotate((preferredRotation + i) * 0.5f * PI))) {
                return (preferredRotation + i) % 4;
            }
        }
        RemoveSelf();
        return 0;
    }
}

[Tracked]
[CustomEntity("GameHelper/McFlammable")]
public class McFlammable : Solid {
    private readonly char tileType;

    public McFlammable(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        tileType = data.Char("tileset", '3');
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(CollideCheck<Player>()) {
            RemoveSelf();
            return;
        }
        TileGrid tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int) Width / 8, (int) Height / 8).TileGrid;
        Add(new LightOcclude());
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
    }
}