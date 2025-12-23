using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities.CCPorts;

public class LoreTabletCutscene(Player player, string dialog, string flag, bool flagValue, string activateSfx, string loopSfx) : CutsceneEntity {
    private readonly Player player = player;
    private readonly string dialog = dialog;
    private readonly string flag = flag;
    private readonly string activateSfx = activateSfx;
    private readonly string loopSfx = loopSfx;
    private readonly bool flagValue = flagValue;
    private LoreText text;
    private EventInstance sound;

    public override void OnBegin(Level level) {
        Add(new Coroutine(Routine()));
    }

    private IEnumerator Routine() {
        SceneAs<Level>().FormationBackdrop.Display = true;
        SceneAs<Level>().FormationBackdrop.Alpha = 0.7f;
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;
        Scene.Add(text = new(dialog, activateSfx));
        yield return text.EaseIn();
        yield return 0.2f;
        sound = Audio.Play(loopSfx);
        yield return text.Wait();
        yield return text.EaseOut();
        Audio.Stop(sound);
        text = null;
        EndCutscene(Level);
    }

    public override void OnEnd(Level level) {
        SceneAs<Level>().FormationBackdrop.Display = false;
        if (!string.IsNullOrEmpty(flag))
            SceneAs<Level>().Session.SetFlag(flag, flagValue);
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        text?.RemoveSelf();
        Audio.Stop(sound);
    }

    private class LoreText : Entity {
        private VirtualRenderTarget target;
        private float alpha = 1f;
        private readonly string text, activateSfx;

        public LoreText(string dialog, string activateSfx) {
            Tag = Tags.HUD;
            Add(new BeforeRenderHook(BeforeRender));
            Position = new(960, 0);
            text = Dialog.Clean(dialog);
            this.activateSfx = activateSfx;
        }

        public IEnumerator EaseIn() {
            for (float p = 0f; p < 1f; p += Engine.DeltaTime) {
                alpha = Ease.CubeOut(p);
                yield return null;
            }
        }

        public IEnumerator Wait() {
            while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed) {
                yield return null;
            }
        }

        public IEnumerator EaseOut() {
            for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.5f) {
                alpha = 1f - Ease.CubeIn(p);
                yield return null;
            }
            RemoveSelf();
        }

        public void BeforeRender() {
            target ??= VirtualContent.CreateRenderTarget("lore-tablet-text", 1920, 1080);
            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ActiveFont.Draw(text, new(960, 200), new Vector2(0.5f, 0.5f), Vector2.One, Color.White * alpha);
            Draw.SpriteBatch.End();
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            Audio.Play(activateSfx);
        }

        public override void Removed(Scene scene) {
            target?.Dispose();
            target = null;
            base.Removed(scene);
        }

        public override void SceneEnd(Scene scene) {
            target?.Dispose();
            target = null;
            base.SceneEnd(scene);
        }

        public override void Render() {
            Level level = Scene as Level;
            if (level?.RetryPlayerCorpse == null && target != null) {
                Draw.SpriteBatch.Draw(target, Position, target.Bounds, Color.White * alpha, 0, new Vector2(target.Width, 0f) / 2f, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}