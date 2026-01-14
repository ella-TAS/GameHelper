using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/CustomBalloon")]
public class CustomBalloon : Entity {
    private static int BalloonCount;
    private readonly Sprite sprite;
    private readonly float floatyOffset, speedYBounce, speedXModifier;
    private readonly bool oneUse, multiplySpeed, refillDash, refillDoubleDash, refillStamina, floaty;
    private readonly string popAudio;

    public CustomBalloon(EntityData data, Vector2 levelOffset, EntityID id) : base(data.Position + levelOffset) {
        oneUse = data.Bool("oneUse");
        speedYBounce = data.Float("speedYBounce");
        multiplySpeed = data.Bool("multiplySpeed");
        speedXModifier = data.Float("speedXModifier");
        refillDash = data.Bool("refillDash");
        refillDoubleDash = data.Bool("refillDoubleDash");
        refillStamina = data.Bool("refillStamina");
        floaty = data.Bool("floaty");
        popAudio = data.Attr("popAudio");
        Collider = Util.ParseHitboxString(data.Attr("hitboxData"), id) ?? new Hitbox(15, 8);
        Depth = data.Int("depth");
        floatyOffset = (int) (-3.15f * GameHelper.Random.NextFloat());
        string spritePath = data.Attr("spriteXmlPath");
        if (spritePath == "red" || spritePath == "blue" || spritePath == "green" || spritePath == "yellow") {
            Add(sprite = GameHelper.SpriteBank.Create("balloon_" + spritePath));
        } else {
            Add(sprite = new Sprite(GFX.Game, spritePath));
        }
        Add(new PlayerCollider(onPlayer));
        sprite.Play("idle", true, true);
    }

    public override void Update() {
        base.Update();
        if (floaty) {
            sprite.RenderPosition = Position + (1.5f * Vector2.UnitY * (float) Math.Sin(2 * (Engine.Scene.TimeActive + floatyOffset)));
        }
    }

    private void onPlayer(Player player) {
        Bounce(player);
        sprite.Play("pop");
        if (popAudio.Length > 0) {
            Audio.Play(popAudio, "balloon_count", BalloonCount);
        }
        if (BalloonCount < 7) {
            BalloonCount++;
        }
        Collidable = false;
        Add(new Coroutine(RoutineRespawn()));
    }

    public void Bounce(Player p) {
        if (p.StateMachine.State == 4 && p.CurrentBooster != null) {
            p.CurrentBooster.PlayerReleased();
            p.CurrentBooster = null;
        }

        Collider collider = base.Collider;
        base.Collider = p.normalHitbox;
        p.MoveVExact((int) (Y - base.Bottom));
        if (!p.Inventory.NoRefills && refillDash) {
            int num = refillDoubleDash ? 2 : 1;
            if (p.Dashes < num) {
                p.Dashes = num;
            }
        }

        if (refillStamina) {
            p.RefillStamina();
        }
        p.StateMachine.State = 0;
        p.jumpGraceTimer = 0f;
        p.varJumpTimer = 0.2f;
        p.AutoJump = true;
        p.AutoJumpTimer = 0f;
        p.dashAttackTimer = 0f;
        p.gliderBoostTimer = 0f;
        p.wallSlideTimer = 1.2f;
        p.wallBoostTimer = 0f;
        p.varJumpSpeed = p.Speed.Y = -Math.Abs(speedYBounce);
        p.launched = false;
        Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
        p.Sprite.Scale = new Vector2(0.6f, 1.4f);
        base.Collider = collider;

        if (multiplySpeed) {
            p.Speed.X *= speedXModifier;
        } else {
            p.Speed.X += speedXModifier * (int) p.Facing;
        }
    }

    private IEnumerator RoutineRespawn() {
        yield return 2.5f;
        if (oneUse) {
            RemoveSelf();
            yield break;
        }
        Collidable = true;
        sprite.Play("spawn");
    }

    private static void PlayerUpdate(Player p) {
        if (p.OnGround()) {
            BalloonCount = 0;
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        BalloonCount = -1;
    }

    public static void Hook() {
        Everest.Events.Player.OnAfterUpdate += PlayerUpdate;
    }

    public static void Unhook() {
        Everest.Events.Player.OnAfterUpdate -= PlayerUpdate;
    }
}