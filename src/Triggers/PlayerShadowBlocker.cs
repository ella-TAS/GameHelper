using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Triggers;

[Tracked]
[CustomEntity("GameHelper/PlayerShadowBlocker")]
public class PlayerShadowBlocker(EntityData data, Vector2 levelOffset) : Trigger(data, levelOffset) { }