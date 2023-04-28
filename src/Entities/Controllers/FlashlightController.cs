using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/FlashlightController")]
public class FlashlightController : Entity {
    private Sprite sprite;
    private Level level;
    private float baseAlpha;
    private float fadeSpeed;
    private float maxCooldown, cooldown;

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

    public override void Update() {
        base.Update();
        cooldown -= Engine.DeltaTime;
        if(Input.MenuJournal && cooldown <= 0) {
            Input.MenuJournal.ConsumePress();
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
            Logger.Log(LogLevel.Warn, "GameHelper", "FlashlightController has bad cooldown value in room " + SceneAs<Level>().Session.LevelData.Name);
            RemoveSelf();
        }
    }
}