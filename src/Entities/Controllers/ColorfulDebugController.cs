using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/ColorfulDebugController")]

public class ColorfulDebugController(EntityData data, Vector2 levelOffset) : Entity(data.Position + levelOffset) {
    // handled in ColorfulDebug.cs

    public override void Added(Scene scene) { }
}