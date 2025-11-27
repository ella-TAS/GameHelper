using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Triggers;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/PlayerShadowController")]
public class PlayerShadowController : Entity {
    private static Dictionary<string, ButtonBinding> keyBinds = new();
    public VirtualButton Binding => keyBinds.TryGetValue(SceneAs<Level>().Session.Area.SID, out ButtonBinding b) ? b.Button : Input.MenuJournal;

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

    internal static void resetBindings() {
        keyBinds = new();
    }

    internal static void addBinding(string levelSID, ButtonBinding binding) {
        if (!keyBinds.TryAdd(levelSID, binding)) {
            Logger.Warn("GameHelper", "PlayerShadowController keybinds already contain key " + levelSID);
            return;
        }
    }
}