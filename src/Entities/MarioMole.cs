using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/MarioMole")]
public class MarioMole : Solid {
    private const float gravity = 7.5f;
    private const float fallCap = 160f;
    private readonly Sprite sprite;
    private readonly float speedX;
    private float velX, velY, jumpTimer;
    private bool movingRight;
    private readonly bool kill, hasGravity;
    private readonly string flag;

    public MarioMole(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, 24, 29, safe: false) {
        speedX = data.Float("speed");
        movingRight = data.Bool("startRight");
        kill = data.Bool("kill");
        flag = data.Attr("flag");
        hasGravity = data.Bool("gravity");
        Position += 3 * Vector2.UnitY;
        Depth = -1;
        sprite = GameHelper.SpriteBank.Create("mario_mole");
        sprite.RenderPosition = new Vector2(-4, -3);
        sprite.FlipY = data.Bool("flipSprite");
        sprite.FlipX = !movingRight;
        Add(sprite);
    }

    public override void Update() {
        base.Update();

        //player kill check
        if(kill) {
            Player p = Scene.Tracker.GetEntity<Player>();
            if(p != null && (p.CollideCheck(this, p.Position + Vector2.UnitX) || p.CollideCheck(this, p.Position - Vector2.UnitX))) {
                p.Die((p.Center - Center).SafeNormalize());
            }
        }

        //x movement
        velX = Calc.Approach(velX, speedX * (movingRight ? 1 : -1), Math.Abs(velX) > Math.Abs(speedX) ? 4.333f : 10.833f);
        if(Util.GetFlag(flag, Scene, true)) {
            bool collided = MoveHCollideSolidsAndBounds(SceneAs<Level>(), velX * Engine.DeltaTime, thruDashBlocks: true);
            if(!collided) {
                foreach(SeekerBarrier sb in SceneAs<Level>().Tracker.GetEntities<SeekerBarrier>()) {
                    if(sb.CollideCheck(this)) {
                        collided = true;
                        MoveH(-velX * Engine.DeltaTime);
                        break;
                    }
                }
            }
            if(CollideCheck<Solid>(Position - 10 * Vector2.UnitX) && CollideCheck<Solid>(Position + 10 * Vector2.UnitX)) {
                sprite.Play("stop");
            } else if(collided) {
                movingRight = !movingRight;
                sprite.FlipX = !movingRight;
                velX = 0f;
            }
        } else {
            velX = 0f;
        }

        //y movement
        jumpTimer -= Engine.DeltaTime;
        if(jumpTimer <= 0) {
            velY = Calc.Approach(velY, fallCap, gravity);
        }
        if(hasGravity || velY < 0f) {
            if(MoveVCollideSolids(velY * Engine.DeltaTime, thruDashBlocks: true)) {
                velY = 0f;
            }
            if(Top > SceneAs<Level>().Bounds.Bottom + 8f) {
                RemoveSelf();
            }
        }

        //spring interaction
        foreach(Spring sp in SceneAs<Level>().Entities.FindAll<Spring>()) {
            if(CollideCheck(sp)) {
                Audio.Play("event:/game/general/spring", sp.BottomCenter);
                sp.staticMover.TriggerPlatform();
                sp.sprite.Play("bounce", restart: true);
                sp.wiggler.Start();
                switch(sp.Orientation) {
                    case Spring.Orientations.Floor:
                        velY = -185f;
                        jumpTimer = 0.2f;
                        break;
                    case Spring.Orientations.WallLeft:
                        velY = -140f;
                        jumpTimer = 0.2f;
                        velX = 240f;
                        movingRight = true;
                        break;
                    case Spring.Orientations.WallRight:
                        velY = -140f;
                        jumpTimer = 0.2f;
                        velX = -240f;
                        movingRight = false;
                        break;
                }
                sprite.FlipX = !movingRight;
                break;
            }
        }
    }
}