using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GameHelper.Entities.Controllers;

[CustomEntity("GameHelper/CameraPeekController")]

public class CameraPeekController : Entity {
    private readonly float magnitudeUp, magnitudeDown, pressTime;
    private Vector2 originalOffset;
    private float pressTimer;
    private bool pressingUp, peekActive;

    public CameraPeekController(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        magnitudeUp = 48f * data.Float("magnitudeUp");
        magnitudeDown = 48f * data.Float("magnitudeDown");
        pressTime = data.Float("pressDuration");
    }

    public override void Update() {
        base.Update();

        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        bool peekAllowed = p != null && p.StateMachine.State == Player.StNormal && p.OnGround() && p.Speed.X == 0f;

        if (peekAllowed && magnitudeUp > 0f && Input.Aim.Value.Y < -0.5f) {
            // up
            if (!pressingUp) pressTimer = 0f;
            pressTimer += Engine.DeltaTime;

            pressingUp = true;
        } else if (peekAllowed && magnitudeDown > 0f && Input.Aim.Value.Y > 0.5f) {
            // down
            if (pressingUp) pressTimer = 0f;
            pressTimer += Engine.DeltaTime;

            pressingUp = false;
        } else {
            pressTimer = 0f;
        }

        if (pressTimer >= pressTime) {
            if (!peekActive) {
                // activate peek
                originalOffset = SceneAs<Level>().CameraOffset;
                SceneAs<Level>().CameraOffset.Y += pressingUp ? -magnitudeUp : magnitudeDown;
            }

            peekActive = true;
        } else {
            if (peekActive) {
                // deactivate peek
                SceneAs<Level>().CameraOffset = originalOffset;
            }

            peekActive = false;
        }
    }
}