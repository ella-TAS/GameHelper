using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Utils;

public class Arrow : Actor {
    private readonly bool facingLeft;

    public Arrow(Vector2 position, bool facingLeft, string sprite_path) : base(position) {
        this.facingLeft = facingLeft;
        Collider = new Hitbox(16, 4);
        Depth = -1;
        Add(new Image(GFX.Game[sprite_path]) {
            FlipX = facingLeft
        });
        Add(new PlayerCollider(onCollide));
    }

    public override void Update() {
        base.Update();
        if(!Collidable) {
            return;
        }
        bool collided = MoveH((facingLeft ? -240 : 240) * Engine.DeltaTime);
        if(collided) {
            Collidable = false;
            Position.X += (facingLeft ? -2 : 2);
            Add(new Coroutine(routineDespawn()));
        }
    }

    private void onCollide(Player p) {
        p.Die(Vector2.UnitX * (facingLeft ? -1 : 1));
    }

    private IEnumerator routineDespawn() {
        yield return 10f;
        RemoveSelf();
    }
}