using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

[CustomEntity("GameHelper/MouseRotator")]
public class MouseRotator : Solid {
    private Vector2 movement;
    public bool Clockwise;

    public MouseRotator(EntityData data, Vector2 levelOffset)
    : base(data.Position + levelOffset, 16, 16, safe: false) {
        Clockwise = data.Bool("clockwise");
        base.OnDashCollide = OnDashed;
        base.Depth = -1;
    }

    public override void Update() {
        base.Update();
        if(movement != Vector2.Zero) {
            bool collideX = MoveHCollideSolidsAndBounds(SceneAs<Level>(), movement.X * 120 * Engine.DeltaTime, false);
            bool collideY = MoveVCollideSolidsAndBounds(SceneAs<Level>(), movement.Y * 120 * Engine.DeltaTime, false, null);
            if(collideX || collideY) {
                movement = Vector2.Zero;
            }
        }
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        scene.Add(new Mouse(Position - 20 * Vector2.UnitY));
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction) {
        movement = direction;
		return DashCollisionResults.Rebound;
	}
}