using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {

    private readonly Vector2 anchor;
    private readonly float ropeLength;
    private float wiggleTimer;

    public SwingSolid(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        anchor = data.Nodes[0];
        ropeLength = (Center - anchor).Length();
    }

    public override void Update() {
        base.Update();

        wiggleTimer += Engine.DeltaTime;
        Center = ;
    }
}