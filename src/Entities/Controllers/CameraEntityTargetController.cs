using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/CameraEntityTargetTrigger", "GameHelper/CameraEntityTargetController")]
public class CameraEntityTargetController : Wrapper {
    private readonly string onlyType, flag;
    private readonly float lerp;
    private readonly bool resetOnFalse, debug;
    private readonly Vector2 offset;

    private Entity target;

    public CameraEntityTargetController(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        onlyType = data.Attr("onlyType");
        flag = data.Attr("flag");
        lerp = data.Float("lerp");
        debug = data.Bool("debug");
        resetOnFalse = data.Bool("resetOnFalse");
        offset = new Vector2(data.Float("offsetX"), data.Float("offsetY"));
        Depth = -20;
    }

    public override void Update() {
        base.Update();
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (p == null) {
            RemoveSelf();
            return;
        }
        if (Util.GetFlag(flag, Scene, true)) {
            findTarget();
            if (target == null) return;
            p.CameraAnchor = target.Center - new Vector2(160, 90) + offset;
            p.CameraAnchorLerp = Vector2.One * lerp;
        } else if (resetOnFalse) {
            p.CameraAnchor = p.CameraAnchorLerp = Vector2.Zero;
        }
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (Util.GetFlag(flag, Scene, true)) findTarget();
    }

    private void findTarget() {
        if (target != null) return;

        target = FindNearest(Position, onlyType);
        if (target == null) {
            ComplainEntityNotFound("Camera Entity Target Trigger");
        } else if (debug) {
            Logger.Info("GameHelper", "Camera Entity Target Trigger found entity " + target.GetType());
        }
    }
}