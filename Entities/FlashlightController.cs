//EllaTAS
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper;

namespace Celeste.Mods.GameHelper.Entities;

[CustomEntity("GameHelper/FlashlightController")]
public class FlashlightController : Entity {
    private static GameHelperModuleSettings settings;
    private Level level;
    private float fadeSpeed;
    private int cooldownTime, cooldown;

    public FlashlightController(EntityData data, Vector2 levelOffset) {
        fadeSpeed = data.Float("fadeSpeed");
        cooldownTime = data.Int("cooldown");
    }

    public override void Update() {
        base.Update();
        if(Input.Talk && cooldown <= 0) {
            Input.Talk.ConsumePress();
            level.Lighting.Alpha = level.Session.DarkRoomAlpha - 1;
            cooldown = cooldownTime;
        }
        cooldown--;
        level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, level.Session.DarkRoomAlpha, fadeSpeed);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        level = SceneAs<Level>();
    }
}