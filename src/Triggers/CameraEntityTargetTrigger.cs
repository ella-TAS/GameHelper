using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Entities.Wrappers;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/CameraEntityTargetTrigger")]
public class CameraEntityTargetTrigger : Wrapper {
    private readonly string onlyType, flag;
    private readonly float lerp;
    private readonly bool debug;
    private readonly Vector2 offset;

    private Entity target;

    public CameraEntityTargetTrigger(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        onlyType = data.Attr("onlyType");
        flag = data.Attr("flag");
        lerp = data.Float("lerp");
        debug = data.Bool("debug");
        offset = new Vector2(data.Float("offsetX"), data.Float("offsetY"));
        Depth = -20;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if(p == null) {
            RemoveSelf();
            return;
        }
        if(Util.GetFlag(flag, Scene, true)) {
            p.CameraAnchor = target.Center - new Vector2(160, 90) + offset;
            p.CameraAnchorLerp = Vector2.One * lerp;
        }
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(debug) LogAllEntities();
        target = FindNearest(Position, onlyType);
        if(target == null) ComplainEntityNotFound("Camera Entity Target Trigger");
        if(debug) Logger.Info("GameHelper", "Camera Entity Target Trigger found entity " + target.GetType());
    }
}