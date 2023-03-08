using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Balloon")]
public class Balloon : Entity {
    private Sprite sprite;
    private int respawnTimer;

    public Balloon(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        base.Collider = new Hitbox(15, 8);
        Add(new PlayerCollider(onCollide));
        Add(sprite = GameHelperModule.getSpriteBank().Create("balloon"));
    }

    public override void Update() {
        base.Update();
        respawnTimer--;
        if(respawnTimer == 0) {
            base.Collidable = true;
            sprite.Play("spawn");
        }
    }

    private void onCollide(Player player) {
        player.Bounce(Position.Y);
        player.Speed.X *= 1.2f;
        base.Collidable = false;
        respawnTimer = 150;
        sprite.Play("pop");
    }
}