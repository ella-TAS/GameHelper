using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Threading;

namespace Celeste.Mod.GameHelper.Utils.Components;

public class ForcefieldComponent : Component {
    public float removeTimer = 0.05f;
    public float entrySpeed;
    public bool enabled = true;
    public TimeRateModifier timeRate;
    private Vector2 forceVec;
    private Player player;
    private Forcefield field;

    public ForcefieldComponent(Forcefield field, float entrySpeed, Vector2 forceVec) : base(true, false) {
        this.entrySpeed = entrySpeed;
        this.forceVec = forceVec;
        this.field = field;
        timeRate = field.timeRateModifier;
    }

    public override void Update() {
        base.Update();

        removeTimer -= Engine.DeltaTime;
        if (removeTimer <= 0f) {
            RemoveSelf();
            return;
        }

        if (enabled) {
            // intentionally not scaled with DeltaTime for the effect
            player.Speed += forceVec;

            if (player.Right < field.Left) {
                timeRate.Multiplier = 1f;
            } else {
                timeRate.Multiplier = Calc.LerpClamp(0.75f, 0.25f, (player.Right - field.Left) / field.Width);
            }
        }
    }

    public override void Added(Entity entity) {
        base.Added(entity);
        player = (Player) entity;
    }

    public void ResetTimer() {
        if (enabled) {
            removeTimer = 0.1f;
        }
    }

    public void Disable() {
        enabled = false;
        field.Collidable = false;
        timeRate.Multiplier = 1f;
    }
}