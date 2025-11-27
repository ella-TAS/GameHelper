using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/HangingBridge")]
public class HangingBridge : Entity {
    private readonly Vector2 endVector;
    private readonly float parabolaExtremity;

    public HangingBridge(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        parabolaExtremity = data.Float("parabolaExtremity");

        endVector = data.Nodes[0] + levelOffset;
        if (endVector.X < X) {
            // switch nodes so end is always right
            Vector2 newPos = endVector;
            endVector = Position;
            Position = newPos;
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        if (parabolaExtremity == 0) {
            Logger.Warn("GameHelper", "parabolaExtremity must be non-zero in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
            return;
        }

        float x2 = endVector.X;
        float y2 = endVector.Y;
        float k = -parabolaExtremity / 100f;

        // parabola y = k (x - vertex)² + offset
        // <=> v = ( x₂²k - x₁²k + y₁ - y₂ ) / ( 2k(x₂ - x₁) )
        float q = 2 * (x2 - X);
        float vertex = (x2 * x2 - X * X) / q + (Y - y2) / (k * q);
        float offset = Y - k * (X - vertex) * (X - vertex);

        // create a segment for every pixel until the end position
        for (Vector2 current = Position; current.X < x2; current.X += 1f) {
            current.Y = k * (current.X - vertex) * (current.X - vertex) + offset;
            SceneAs<Level>().Add(new RopeSegment(current));
        }
    }
}