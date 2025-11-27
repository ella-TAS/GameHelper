using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Utils;

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
        if (freezeFrames) Celeste.Freeze(0.05f);
        if (oneUse) {
            Image disperse = Get<Image>();
            SceneAs<Level>().Add(new DisperseImage(Position + renderOffset, Vector2.UnitY, disperse.Origin, Vector2.One, disperse.Texture));
            RemoveSelf();
        }
        p.Speed.X *= 1.2f;
        if (clipToTop) {
            p.Bounce(Top + 2f);
        } else {
            p.Bounce(p.Y);
            Collidable = false;
            Add(new Coroutine(uncollideRoutine()));
        }
    }

    private IEnumerator uncollideRoutine() {
        while (CollideCheck<Player>()) {
            yield return null;
        }
        Collidable = true;
    }
}