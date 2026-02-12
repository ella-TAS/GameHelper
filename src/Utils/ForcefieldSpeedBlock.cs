using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

public class ForcefieldSpeedBlock : Solid {
    private Image key;
    private MTexture[,] nineSlice;

    public ForcefieldSpeedBlock(Vector2 position, int height) : base(position, 16, height, safe: false) {
        OnDashCollide = onDashed;
        key = new Image(GFX.Game["collectables/key/idle00"]);
        key.CenterOrigin();
        key.Position = new Vector2(Width / 2f, Height / 2f);
        MTexture mTexture = GFX.Game["objects/goldblock"];
        nineSlice = new MTexture[3, 3];
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                nineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
            }
        }
        Depth = -10000;
        SurfaceSoundIndex = 32;
        Tag = Tags.FrozenUpdate;
    }

    private DashCollisionResults onDashed(Player player, Vector2 direction) {
        if (player.Get<ForcefieldComponent>() is ForcefieldComponent component && component.enabled) {
            component.Disable();
            player.Speed.X = Calc.Max(component.entrySpeed * 0.8f, 280f);
            player.Speed.Y = -150f;
            explodeLaunch(player);
            key = new Image(GFX.Game["objects/GameHelper/forcefield/key_glow"]);
            key.CenterOrigin();
            key.Position = new Vector2(Width / 2f, Height / 2f);
            Add(new Coroutine(RoutineRemove()));
            return DashCollisionResults.Ignore;
        }
        return DashCollisionResults.NormalCollision;
    }

    private IEnumerator RoutineRemove() {
        // remove after the freeze frames
        yield return null;
        RemoveSelf();
    }

    private static void explodeLaunch(Player p) {
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        Celeste.Celeste.Freeze(0.2f);
        p.launchApproachX = null;
        p.AutoJump = true;
        p.explodeLaunchBoostTimer = 0f;
        p.explodeLaunchBoostSpeed = p.Speed.X;
        SlashFx.Burst(p.Center, p.Speed.Angle());
        p.RefillStamina();
        p.dashCooldownTimer = 0.2f;
        p.StateMachine.State = 7;
    }

    private void DrawBlock(Vector2 offset, Color color) {
        float num = Width / 8f - 1f;
        float num2 = Height / 8f - 1f;
        for (int i = 0; i <= num; i++) {
            for (int j = 0; j <= num2; j++) {
                int num3 = i < num ? Math.Min(i, 1) : 2;
                int num4 = j < num2 ? Math.Min(j, 1) : 2;
                nineSlice[num3, num4].Draw(Position + offset + base.Shake + new Vector2(i * 8, j * 8), Vector2.Zero, color);
            }
        }
    }

    public override void Render() {
        // outline
        DrawBlock(new Vector2(-1f, 0f), Color.Black);
        DrawBlock(new Vector2(1f, 0f), Color.Black);
        DrawBlock(new Vector2(0f, -1f), Color.Black);
        DrawBlock(new Vector2(0f, 1f), Color.Black);
        DrawBlock(Vector2.Zero, Color.White);

        // key icon
        key.Color = Color.White;
        key.RenderPosition = base.Center;
        key.Render();
    }
}