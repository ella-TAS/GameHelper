using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FlashlightController")]
public class FlashlightController : Entity {
    private static Dictionary<string, ButtonBinding> keyBinds = new();
    public VirtualButton Binding => keyBinds.TryGetValue(SceneAs<Level>().Session.Area.SID, out ButtonBinding b) ? b.Button : Input.MenuJournal;

    private readonly Sprite sprite;
    private Level level;
    private float baseAlpha;
    private readonly float fadeSpeed;
    private readonly float maxCooldown;
    private float cooldown;

#pragma warning disable IDE0060, RCS1163
    public FlashlightController(EntityData data, Vector2 levelOffset) {
        fadeSpeed = 1 / data.Float("fadeTime");
        maxCooldown = data.Float("cooldown");
        sprite = new Sprite(GFX.Gui, "GameHelper/");
        sprite.AddLoop("idle", "flashlight", 1f);
        sprite.Play("idle");
        sprite.Visible = false;
        Add(sprite);
        base.Tag = Tags.HUD;
        base.Position = new Vector2(1800, 960);
    }
#pragma warning restore

    public override void Update() {
        base.Update();
        cooldown -= Engine.DeltaTime;
        if(Binding.Pressed && cooldown <= 0) {
            Binding.ConsumePress();
            level.Lighting.Alpha = 0;
            cooldown = maxCooldown;
            sprite.Visible = true;
            Audio.Play("event:/GameHelper/Flashlight");
        } else if(cooldown <= 0) {
            sprite.Visible = false;
        }
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, baseAlpha, fadeSpeed * Engine.DeltaTime);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        level = SceneAs<Level>();
        baseAlpha = level.DarkRoom ? level.Session.DarkRoomAlpha : level.BaseLightingAlpha;
        if(maxCooldown <= 0) {
            Logger.Warn("GameHelper", "FlashlightController has bad cooldown value in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
    }

    public static void resetBindings() {
        keyBinds = new();
    }

    public static void addBinding(string levelSID, ButtonBinding binding) {
        if(keyBinds.ContainsKey(levelSID)) {
            Logger.Warn("GameHelper", "FlashlightController keybinds already contain key " + levelSID);
            return;
        }
        keyBinds.Add(levelSID, binding);
    }
}