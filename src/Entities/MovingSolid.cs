using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/MovingSolid")]
public class MovingSolid : Solid {
    private readonly float moveTime;
    private readonly string easeMode;
    private readonly int width, height;
    private readonly char tileType;
    private float currentTime;
    private Vector2 homePos, targetPos;

    public MovingSolid(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: true) {
        moveTime = data.Float("moveTime");
        easeMode = data.Attr("easeMode");
        tileType = data.Char("tileset", '3');
        width = data.Width;
        height = data.Height;
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        homePos = Position;
        targetPos = data.Nodes[0] + levelOffset;
    }

    public override void Update() {
        base.Update();

        currentTime += Engine.DeltaTime;
        if(currentTime >= moveTime) {
            currentTime = 0;
            Vector2 swap = targetPos;
            targetPos = homePos;
            homePos = swap;
        }

        float withEase = Util.EaseMode(currentTime / moveTime, easeMode);
        Vector2 target = homePos + (targetPos - homePos) * withEase;
        MoveTo(target);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        TileGrid tileGrid = GFX.FGAutotiler.GenerateBox(tileType, width / 8, height / 8).TileGrid;
        Add(new LightOcclude());
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
    }
}