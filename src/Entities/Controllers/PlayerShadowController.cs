using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PlayerShadowController")]
public class PlayerShadowController : Entity {
    private int uses;
    private readonly bool oneUse, freezeFrames, clipToTop;
    private readonly string texture;

    public PlayerShadowController(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        texture = data.Attr("sprite", "objects/GameHelper/player_shadow");
        Depth = -9;
        uses = data.Int("uses");
        freezeFrames = data.Bool("freezeFrames");
        clipToTop = data.Bool("clipToTop");
    }

    public override void Update() {
        base.Update();
        if((uses > 0 || uses < 0) && Input.MenuJournal.Pressed) {
            Input.MenuJournal.ConsumeBuffer();
            Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
            if(p != null) {
                SceneAs<Level>().Add(new PlayerShadow(p.TopLeft, texture, oneUse, freezeFrames, clipToTop));
                uses--;
            }
        }
    }
}

public class PlayerShadow : Entity {
    private readonly bool oneUse, freezeFrames, clipToTop;
    private readonly Vector2 renderOffset;

    public PlayerShadow(Vector2 position, string texture, bool oneUse, bool freezeFrames, bool clipToTop) : base(position) {
        renderOffset = new(-2, -2);

        Collider = new Hitbox(8, 9);
        Add(new PlayerCollider(onPlayer));
        Add(new Image(GFX.Game[texture]) {
            RenderPosition = renderOffset
        });
        Depth = -10;
        this.oneUse = oneUse;
        this.freezeFrames = freezeFrames;
        this.clipToTop = clipToTop;
    }

    private void onPlayer(Player p) {
        Audio.Play("event:/game/general/thing_booped", Position);
        if(freezeFrames) Celeste.Freeze(0.05f);
        if(oneUse) {
            Image disperse = Get<Image>();
            SceneAs<Level>().Add(new DisperseImage(Position + renderOffset, Vector2.UnitY, disperse.Origin, Vector2.One, disperse.Texture));
            RemoveSelf();
        }
        p.Speed.X *= 1.2f;
        if(clipToTop) {
            p.Bounce(Top + 2f);
        } else {
            p.Bounce(p.Y);
            Collidable = false;
            Add(new Coroutine(uncollideRoutine()));
        }
    }

    private IEnumerator uncollideRoutine() {
        while(CollideCheck<Player>()) {
            yield return null;
        }
        Collidable = true;
    }
}