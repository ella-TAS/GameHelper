//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/FlashlightController")]
public class FlashlightController : Entity {
    private Level level;
    private float baseAlpha;
    private float fadeSpeed;
    private int _cooldown, cooldown;

    public FlashlightController(EntityData data, Vector2 levelOffset) {
        fadeSpeed = 1 / data.Float("fadeTime");
        _cooldown = data.Int("cooldown");
    }

    public override void Update() {
        base.Update();
        cooldown--;
        if(Input.Talk && cooldown <= 0) {
            Input.Talk.ConsumePress();
            level.Lighting.Alpha = 0;
            cooldown = _cooldown;
            Audio.Play("event:/GameHelper/Flashlight");
        }
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, baseAlpha, fadeSpeed);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        level = SceneAs<Level>();
        baseAlpha = level.DarkRoom ? level.Session.DarkRoomAlpha : level.BaseLightingAlpha;
        if(_cooldown <= 0) {
            Logger.Log("GameHelper", "FlashlightController has bad cooldown value");
            RemoveSelf();
        }
    }
}