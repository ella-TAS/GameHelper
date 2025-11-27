using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.GameHelper.Entities;

[Tracked]
[CustomEntity("GameHelper/McFlammable")]
public class McFlammable : Solid {
    private readonly char tileType;
    private bool claimed;
    public EntityID id;

    public McFlammable(EntityData data, Vector2 levelOffset, EntityID id) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        tileType = data.Char("tileset", '3');
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        Depth = -10;
        this.id = id;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (CollideCheck<Player>() || CollideAll<McFlammable>().Any(fuel => (fuel as McFlammable)?.id.ID > id.ID)) {
            RemoveSelf();
            return;
        }
        TileGrid tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int) Width / 8, (int) Height / 8).TileGrid;
        Add(new LightOcclude());
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
    }

    public bool Claim() {
        bool wasClaimed = claimed;
        claimed = true;
        return !wasClaimed;
    }
}