using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

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

    public override void Added(Scene scene) {
        base.Added(scene);
        AreaKey area = SceneAs<Level>().Session.Area;
        base.Collidable = !SaveData.Instance.Areas_Safe[area.ID].Modes[(int) area.Mode].HeartGem;
    }
}