using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/RegisterHeartTrigger")]
public class RegisterHeart : Trigger {
    public RegisterHeart(EntityData data, Vector2 levelOffset) : base(data, levelOffset) { }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        SaveData.Instance.RegisterHeartGem(level.Session.Area);
        level.AutoSave();
        base.Collidable = false;
    }
}