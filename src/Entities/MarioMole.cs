using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/MarioMole")]
[Tracked]
public class MarioMole : Solid {
    private const float fallCap = 200f;
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
        Depth = 1;
        sprite = GameHelper.SpriteBank.Create("mario_mole");
        sprite.RenderPosition = new Vector2(-4, -3);
        sprite.FlipY = data.Bool("flipSprite");
        sprite.FlipX = !movingRight;
        Add(sprite);
    }

    public override void Update() {
        //x movement
        velX = Calc.Approach(velX, speedX * (movingRight ? 1 : -1), Math.Abs(velX) > Math.Abs(speedX) ? 4.333f : 10.833f);
        if(Util.GetFlag(flag, Scene, true)) {
            bool collided = MoveHor(velX * Engine.DeltaTime);
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
            sprite.Play("stop");
        }

        //y movement
        jumpTimer -= Engine.DeltaTime;
        if(jumpTimer <= 0) {
            velY = Calc.Approach(velY, fallCap, Math.Abs(velY) <= 40f ? 7.5f : 15f);
        }
        if(hasGravity || velY < 0f) {
            if(MoveVer(velY * Engine.DeltaTime)) {
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
                        if(velY >= 0f) {
                            velX *= 0.5f;
                            velY = -185f;
                            jumpTimer = 0.15f;
                        }
                        break;
                    case Spring.Orientations.WallLeft:
                        if(velX <= 0f) {
                            velX = 220f;
                            velY = -80f;
                            jumpTimer = 0.1f;
                            movingRight = true;
                        }
                        break;
                    case Spring.Orientations.WallRight:
                        if(velX >= 0f) {
                            velX = -220f;
                            velY = -80f;
                            jumpTimer = 0.1f;
                            movingRight = false;
                        }
                        break;
                }
                sprite.FlipX = !movingRight;
                break;
            }
        }

        base.Update();
    }

    public bool MoveHor(float speedDt) {
        return MoveHCollideSolidsAndBounds(SceneAs<Level>(), speedDt, thruDashBlocks: true);
    }

    public bool MoveVer(float speedDt) {
        return MoveVCollideSolids(speedDt, thruDashBlocks: true);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        if(!kill) return;
        Spikes s;
        s = new(
            TopLeft + new Vector2(2, 4),
            (int) Height - 4,
            Spikes.Directions.Left,
            "default"
        );
        s.Visible = false;
        SceneAs<Level>().Add(s);
        Add(new EntityMoveComponent(s, removeCascade: true));
        s = new(
            TopRight + new Vector2(-2, 4),
            (int) Height - 4,
            Spikes.Directions.Right,
            "default"
        );
        s.Visible = false;
        SceneAs<Level>().Add(s);
        Add(new EntityMoveComponent(s, removeCascade: true));
    }

    public static void Hook() {
        On.Celeste.Solid.MoveHExact += OnSolidMoveHExact;
        On.Celeste.Solid.MoveVExact += OnSolidMoveVExact;
    }

    public static void Unhook() {
        On.Celeste.Solid.MoveHExact -= OnSolidMoveHExact;
        On.Celeste.Solid.MoveVExact -= OnSolidMoveVExact;
    }

    //move mole above any platform
    private static void OnSolidMoveHExact(On.Celeste.Solid.orig_MoveHExact orig, Solid self, int movedPx) {
        foreach(MarioMole mole in self.CollideAll<MarioMole>(self.Position - 3 * Vector2.UnitY)) {
            if(mole.Bottom > self.Position.Y) {
                continue;
            }
            mole.MoveHor(movedPx);
        }
        orig(self, movedPx);
    }

    private static void OnSolidMoveVExact(On.Celeste.Solid.orig_MoveVExact orig, Solid self, int movedPx) {
        foreach(MarioMole mole in self.CollideAll<MarioMole>(self.Position - (movedPx + 3) * Vector2.UnitY)) {
            if(mole.Bottom > self.Position.Y) {
                continue;
            }
            mole.MoveVer(movedPx);
        }
        orig(self, movedPx);
    }
}