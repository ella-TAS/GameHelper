using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PlayerShadowController")]
public class PlayerShadowController : Entity {
    private int uses;
    private readonly bool oneUse;
    private readonly string texture;

    public PlayerShadowController(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        texture = data.Attr("sprite", "objects/GameHelper/player_shadow");
        Depth = -9;
        uses = data.Int("uses");
    }

    public override void Update() {
        base.Update();
        if((uses > 0 || uses < 0) && Input.MenuJournal.Pressed) {
            Input.MenuJournal.ConsumeBuffer();
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null) {
                SceneAs<Level>().Add(new PlayerShadow(p.TopLeft, texture, oneUse));
                uses--;
            }
        }
    }
}

public class PlayerShadow : Entity {
    private readonly bool oneUse;
    private readonly Vector2 renderOffset;

    public PlayerShadow(Vector2 position, string texture, bool oneUse) : base(position) {
        renderOffset = new(-2, -2);

        Collider = new Hitbox(8, 9);
        Add(new PlayerCollider(onPlayer));
        Add(new Image(GFX.Game[texture]) {
            RenderPosition = renderOffset
        });
        Depth = -10;
        this.oneUse = oneUse;
    }

    private void onPlayer(Player p) {
        Audio.Play("event:/game/general/thing_booped", Position);
        Celeste.Freeze(0.05f);
        p.Bounce(Top + 2f);
        p.Speed.X *= 1.2f;
        if(oneUse) {
            Image disperse = Get<Image>();
            SceneAs<Level>().Add(new DisperseImage(Position + renderOffset, Vector2.UnitY, disperse.Origin, Vector2.One, disperse.Texture));
            RemoveSelf();
        }
    }
}