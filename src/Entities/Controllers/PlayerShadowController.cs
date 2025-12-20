using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Triggers;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/PlayerShadowController")]
public class PlayerShadowController : Entity {
    public static VirtualButton Binding => Input.MenuJournal;

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
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (uses != 0 && Binding.Pressed && !(p?.CollideCheck<PlayerShadowBlocker>() ?? true)) {
            Binding.ConsumeBuffer();
            SceneAs<Level>().Add(new PlayerShadow(p.TopLeft, texture, oneUse, freezeFrames, clipToTop));
            uses--;
        }
    }
}