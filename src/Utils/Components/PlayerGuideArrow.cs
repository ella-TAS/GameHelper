using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class PlayerGuideArrow(Vector2 target, float duration, int renderIndex) : Component(true, true) {
    private readonly Vector2 target = target;
    private readonly int renderIndex = renderIndex;
    private float duration = duration;
    private Image img;

    public override void Update() {
        base.Update();
        if(duration > 0) {
            duration -= Engine.DeltaTime;
            if(duration <= 0) {
                RemoveSelf();
            }
        }
    }

    public override void Render() {
        base.Render();
        img.Position = Entity.Position + new Vector2(0, -24) + renderIndex * new Vector2(0, -16);
        img.Rotation = (target - Entity.Position).Angle();
        img.DrawOutline(new Color(90, 60, 35));
        img.Render();
    }

    public override void Added(Entity entity) {
        base.Added(entity);
        img = new(GFX.Game["objects/GameHelper/guide_arrow"]);
        img.JustifyOrigin(new Vector2(0.5f, 0.5f));
    }
}