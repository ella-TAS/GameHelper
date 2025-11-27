using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/MovingSolid")]
public class MovingSolid : Solid {
    private readonly float moveTime, pauseDuration;
    private readonly string easeMode;
    private readonly char tileType;
    private float currentTime, stopTime;
    private Vector2 homePos, targetPos;

    public MovingSolid(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, data.Width, data.Height, safe: true) {
        moveTime = data.Float("moveTime");
        easeMode = data.Attr("easeMode");
        tileType = data.Char("tileset", '3');
        pauseDuration = data.Float("pauseTime");
        stopTime = data.Float("startOffset");
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        homePos = Position;
        targetPos = data.Nodes[0] + levelOffset;
    }

    public override void Update() {
        base.Update();

        if (stopTime > 0) {
            stopTime -= Engine.DeltaTime;
            return;
        }

        //movement
        currentTime += Engine.DeltaTime;
        if (currentTime >= moveTime) {
            currentTime = 0;
            stopTime = pauseDuration;
            (targetPos, homePos) = (homePos, targetPos);
        }

        float withEase = Util.EaseMode(currentTime / moveTime, easeMode);
        Vector2 target = homePos + (targetPos - homePos) * withEase;
        MoveTo(target);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        TileGrid tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int) Width / 8, (int) Height / 8).TileGrid;
        Add(new LightOcclude());
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
    }
}