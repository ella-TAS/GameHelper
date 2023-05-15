using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/RegisterHeartTrigger")]
public class RegisterHeartTrigger : Trigger {
    private string flag;
    private bool hideAnimation;

    public RegisterHeartTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        flag = data.Attr("flag");
        hideAnimation = data.Bool("hideAnimation");
    }

    public override void OnStay(Player player) {
        if(flag != "" && !SceneAs<Level>().Session.GetFlag(flag)) {
            return;
        }
        if(GameHelper.Instance.Session.HeartTriggerActivated) {
            RemoveSelf();
            return;
        }
        base.Collidable = false;
        Level level = SceneAs<Level>();
        if(!SaveData.Instance.Areas_Safe[level.Session.Area.ID].Modes[(int) level.Session.Area.Mode].HeartGem) {
            SaveData.Instance.RegisterHeartGem(level.Session.Area);
            level.AutoSave();
        }
        if(!hideAnimation) {
            for(int i = 0; i < 25; i++) {
                Scene.Add(new AbsorbOrb(player.Center));
            }
        }
        GameHelper.Instance.Session.HeartTriggerActivated = true;
    }
}