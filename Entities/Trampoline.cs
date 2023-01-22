//EllaTAS
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Trampoline")]
public class Trampoline : Entity {
    private float speedBoost;
    private bool facingUpLeft;
    private bool refillDash;
    private bool oneUse;
    private int hasCollided = 0;
    private int collidable = 0;
    private Sprite sprite;

    public Trampoline(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        speedBoost = data.Float("speedBoost");
        facingUpLeft = data.Bool("facingUpLeft");
        refillDash = data.Bool("refillDash");
        oneUse = data.Bool("oneUse");
        base.Collider = new ColliderList(new Hitbox(16f, 16f, 0f, 0f));
        Add(new PlayerCollider(onCollide));
        Add(sprite = GameHelperModule.getSpriteBank().Create("trampoline"));
    }

    private void onCollide(Player player) {
        if(collidable == 0) {
            float speedX = player.Speed.X;
            player.Speed.X = (player.Speed.Y + speedBoost) * (facingUpLeft ? -1 : 1);
            player.Speed.Y = speedX * (facingUpLeft ? -1 : 1) - speedBoost;
        }
        collidable++;
        if(oneUse) {
            base.Collidable = false;
        }
    }

    public override void Update() {
        base.Update();
        if(collidable != 0) {
            hasCollided++;
            if(collidable != hasCollided) {
                collidable = hasCollided = 0;
            }
        }
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(collidable == 1){
            p.StateMachine.State = 0;
			if (!p.Inventory.NoRefills && refillDash) {
                p.RefillDash();
            }
			p.RefillStamina();
        }
    }
}