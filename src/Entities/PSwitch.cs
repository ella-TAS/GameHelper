using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PSwitch")]
public class PSwitch : Actor {
    private static ParticleType P_Impact;
    private readonly Sprite sprite;
    private readonly Holdable Hold;
    private readonly JumpThru platform;
    private readonly Collision CollideH, CollideV;
    private readonly bool showTutorial, stationary;
    private readonly string flag;
    private readonly float flagDuration;
    private Vector2 Speed, prevLiftSpeed;
    private BirdTutorialGui tutorialGui;
    private float tutorialTimer, hardVerticalHitSoundCooldown, noGravityTimer;
    private bool dead, pressed;

    public PSwitch(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        P_Impact = new() {
            Color = Calc.HexToColor("566be2"),
            Size = 1f,
            FadeMode = ParticleType.FadeModes.Late,
            DirectionRange = 1.74532926f,
            SpeedMin = 10f,
            SpeedMax = 20f,
            SpeedMultiplier = 0.1f,
            LifeMin = 0.3f,
            LifeMax = 0.8f
        };
        stationary = data.Bool("stationary");
        showTutorial = data.Bool("showTutorial");
        flagDuration = data.Float("flagDuration");
        flag = data.Attr("flag");
        Depth = 100;
        Add(sprite = GameHelper.SpriteBank.Create("p_switch_" + data.Attr("sprite", "blue")));
        sprite.RenderPosition += new Vector2(-10, -21);
        platform = new JumpThru(Position + new Vector2(-8, -16), 16, false) {
            SurfaceSoundIndex = 32
        };
        Collider = new Hitbox(8f, 10f, -4f, -10f);
        Add(new VertexLight(Collider.Center, Color.White, 1f, 32, 64));

        if(stationary) return;
        Add(Hold = new() {
            PickupCollider = new Hitbox(16f, 22f, -8f, -16f),
            SlowFall = false,
            SlowRun = true,
            OnPickup = OnPickup,
            OnRelease = OnRelease,
            OnHitSpring = HitSpring,
            SpeedGetter = () => Speed,
            SpeedSetter = (speed) => Speed = speed
        });
        CollideH = OnCollideH;
        CollideV = OnCollideV;
        LiftSpeedGraceTime = 0.1f;
    }

    public override void Update() {
        base.Update();
        if(dead) return;
        hardVerticalHitSoundCooldown -= Engine.DeltaTime;
        if(Hold?.IsHeld ?? false) {
            prevLiftSpeed = Vector2.Zero;
        } else if(!stationary) {
            if(OnGround()) {
                float target = (!OnGround(Position + Vector2.UnitX * 3f)) ? 20f : (OnGround(Position - Vector2.UnitX * 3f) ? 0f : (-20f));
                Speed.X = Calc.Approach(Speed.X, target, 800f * Engine.DeltaTime);
                Vector2 liftSpeed = LiftSpeed;
                if(liftSpeed == Vector2.Zero && prevLiftSpeed != Vector2.Zero) {
                    Speed = prevLiftSpeed;
                    prevLiftSpeed = Vector2.Zero;
                    Speed.Y = Math.Min(Speed.Y * 0.6f, 0f);
                    if(Speed.X != 0f && Speed.Y == 0f) Speed.Y = -60f;
                    if(Speed.Y < 0f) noGravityTimer = 0.15f;
                } else {
                    prevLiftSpeed = liftSpeed;
                    if(liftSpeed.Y < 0f && Speed.Y < 0f) Speed.Y = 0f;
                }
            } else if(Hold.ShouldHaveGravity) {
                float accelY = 800f;
                if(Math.Abs(Speed.Y) <= 30f) accelY *= 0.5f;
                float accelX = 350f;
                if(Speed.Y < 0f) accelX *= 0.5f;
                Speed.X = Calc.Approach(Speed.X, 0f, accelX * Engine.DeltaTime);
                if(noGravityTimer > 0f) noGravityTimer -= Engine.DeltaTime;
                else Speed.Y = Calc.Approach(Speed.Y, 200f, accelY * Engine.DeltaTime);
            }
            MoveH(Speed.X * Engine.DeltaTime, CollideH);
            MoveV(Speed.Y * Engine.DeltaTime, CollideV);
            Rectangle bounds = SceneAs<Level>().Bounds;
            if(Right > bounds.Right) {
                Right = bounds.Right;
                Speed.X *= -0.4f;
            } else if(Left < bounds.Left) {
                Left = bounds.Left;
                Speed.X *= -0.4f;
            } else if(Top < bounds.Top - 4) {
                Top = bounds.Top + 4;
                Speed.Y = 0f;
            } else if(Top > bounds.Bottom) Die();
        }

        if(pressed || dead) return;
        Hold?.CheckAgainstColliders();
        if(!stationary && tutorialGui != null) {
            if(!Hold.IsHeld && OnGround()) tutorialTimer += Engine.DeltaTime;
            else tutorialTimer = 0f;
            tutorialGui.Open = tutorialTimer > 0.25f;
        }
        //platform
        if(platform.HasPlayerRider()) {
            pressed = true;
            Hold?.RemoveSelf();
            if(tutorialGui != null) tutorialGui.Open = false;
            Add(new Coroutine(pressRoutine()));
            if(flagDuration > 0) Add(new Coroutine(unflagRoutine()));
        }
        platform.MoveTo(Position + new Vector2(-8, -16));
    }

    private IEnumerator pressRoutine() {
        platform.Collidable = false;
        sprite.Play("pressed");
        SceneAs<Level>().Session.SetFlag(flag);
        Audio.Play("event:/game/05_mirror_temple/button_activate", Position);
        yield return 1f;
        SceneAs<Level>().Add(new DisperseImage(Position + new Vector2(-10, -21), Vector2.UnitY, sprite.Origin, Vector2.One, sprite.Texture));
        yield return null;
        Visible = false;
    }

    private IEnumerator unflagRoutine() {
        yield return flagDuration;
        SceneAs<Level>().Session.SetFlag(flag, false);
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        scene.Add(platform);
        if(showTutorial) {
            scene.Add(tutorialGui = new BirdTutorialGui(
                this, new Vector2(0f, -24f), Dialog.Clean("tutorial_carry"), Dialog.Clean("tutorial_hold"), BirdTutorialGui.ButtonPrompt.Grab) {
                Open = false
            });
        }
    }

    public override void Removed(Scene scene) {
        SceneAs<Level>().Session.SetFlag(flag, false);
        base.Removed(scene);
    }

    private bool HitSpring(Spring spring) {
        if(Hold.IsHeld) return false;
        if(spring.Orientation == Spring.Orientations.Floor && Speed.Y >= 0f) {
            Speed.X *= 0.5f;
            Speed.Y = -160f;
            noGravityTimer = 0.15f;
        } else if(spring.Orientation == Spring.Orientations.WallLeft && Speed.X <= 0f) {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            Speed.X = 220f;
            Speed.Y = -80f;
            noGravityTimer = 0.1f;
        } else if(spring.Orientation == Spring.Orientations.WallRight && Speed.X >= 0f) {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            Speed.X = -220f;
            Speed.Y = -80f;
            noGravityTimer = 0.1f;
        } else return false;
        return true;
    }

    private void OnCollideH(CollisionData data) {
        if(data.Hit is DashSwitch) (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitX * Math.Sign(Speed.X));
        Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", Position);
        if(Math.Abs(Speed.X) > 100f) ImpactParticles(data.Direction);
        Speed.X *= -0.4f;
    }

    private void OnCollideV(CollisionData data) {
        if(data.Hit is DashSwitch) (data.Hit as DashSwitch).OnDashCollide(null, Vector2.UnitY * Math.Sign(Speed.Y));
        if(Speed.Y > 0f) {
            if(hardVerticalHitSoundCooldown <= 0f) {
                Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", Position, "crystal_velocity", Calc.ClampedMap(Speed.Y, 0f, 200f));
                hardVerticalHitSoundCooldown = 0.5f;
            } else Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_ground", Position, "crystal_velocity", 0f);
        }
        if(Speed.Y > 160f) ImpactParticles(data.Direction);
        if(Speed.Y > 140f && data.Hit is not SwapBlock && data.Hit is not DashSwitch) Speed.Y *= -0.6f;
        else Speed.Y = 0f;
    }

    private void ImpactParticles(Vector2 dir) {
        float direction;
        Vector2 position;
        Vector2 positionRange;
        if(dir.X > 0f) {
            direction = MathF.PI;
            position = new Vector2(Right, Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        } else if(dir.X < 0f) {
            direction = 0f;
            position = new Vector2(Left, Y - 4f);
            positionRange = Vector2.UnitY * 6f;
        } else if(dir.Y > 0f) {
            direction = -MathF.PI / 2f;
            position = new Vector2(X, Bottom);
            positionRange = Vector2.UnitX * 10f;
        } else {
            direction = MathF.PI / 2f;
            position = new Vector2(X, Top);
            positionRange = Vector2.UnitX * 10f;
        }
        SceneAs<Level>().ParticlesFG.Emit(P_Impact, 6, position, positionRange, direction);
    }

    public override bool IsRiding(Solid solid) {
        return Speed.Y == 0f && base.IsRiding(solid);
    }

    public override void OnSquish(CollisionData data) {
        if(!TrySquishWiggle(data, 3, 3)) Die();
    }

    private void OnPickup() {
        Speed = Vector2.Zero;
        AddTag(Tags.Persistent);
    }

    private void OnRelease(Vector2 force) {
        RemoveTag(Tags.Persistent);
        if(force.X != 0f && force.Y == 0f) force.Y = -0.4f;
        Speed = force * 200f;
        if(Speed != Vector2.Zero) noGravityTimer = 0.1f;
    }

    public void Die() {
        if(dead) return;
        dead = true;
        Audio.Play("event:/char/madeline/death", Position);
        Add(new DeathEffect(new Color(86, 107, 226), Center - Position));
        Hold.RemoveSelf();
        Collidable = sprite.Visible = AllowPushing = false;
        Depth = -1000000;
    }

    private static void OnPlayerTransition(On.Celeste.Player.orig_OnTransition orig, Player p) {
        orig(p);
        if(p.Holding?.Entity is PSwitch) {
            PSwitch ps = p.Holding.Entity as PSwitch;
            p.SceneAs<Level>().Add(new DisperseImage(ps.Position + new Vector2(-10, -21), -Vector2.UnitY, ps.sprite.Origin, Vector2.One, ps.sprite.Texture));
            ps.RemoveSelf();
        }
    }

    public static void Hook() {
        On.Celeste.Player.OnTransition += OnPlayerTransition;
    }

    public static void Unhook() {
        On.Celeste.Player.OnTransition -= OnPlayerTransition;
    }
}