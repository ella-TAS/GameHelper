using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PlayerShadowController")]
public class PlayerShadowController : Entity {
    private int uses;

    public PlayerShadowController(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        uses = data.Int("uses");
        Depth = -9;
    }

    public override void Update() {
        base.Update();
        if(uses > 0 && Input.MenuJournal.Pressed) {
            Input.MenuJournal.ConsumeBuffer();
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null) {
                SceneAs<Level>().Add(new PlayerShadow(p.TopLeft));
                uses--;
            }
        }
    }
}

public class PlayerShadow : Entity {
    public PlayerShadow(Vector2 position) : base(position) {
        Collider = new Hitbox(8, 9);
        Add(new PlayerCollider(onPlayer));
        Depth = -10;
    }

    private void onPlayer(Player p) {
        Audio.Play("event:/game/general/thing_booped", Position);
        Celeste.Freeze(0.05f);
        p.Bounce(Top + 2f);
        p.Speed.X *= 1.2f;
    }
}