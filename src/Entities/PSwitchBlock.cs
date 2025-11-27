using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PSwitchBlock")]
public class PSwitchBlock : DashBlock {
    private readonly Sprite coinSprite;
    private readonly string flag;
    private readonly bool startBlock;
    private bool isBlock => startBlock ^ Util.GetFlag(flag, Scene) && !collected;
    private bool collected;

    public PSwitchBlock(EntityData data, Vector2 levelOffset, EntityID id)
        : base(data.Position + levelOffset, data.Char("tiletype", '3'), data.Width, data.Height, data.Bool("blendIn"), data.Bool("permanent"), data.Bool("canDash"), id) {
        flag = data.Attr("flag");
        startBlock = data.Bool("startAsBlock");
        OnDashCollide = delegate (Player p, Vector2 dir) {
            if (!canDash) return DashCollisionResults.NormalCollision;
            Break(p.Center, dir, true, true);
            return DashCollisionResults.Ignore;
        };
        coinSprite = GameHelper.SpriteBank.Create("mario_coin_" + data.Attr("coinSprite", "blue"));
        Add(coinSprite);
    }

    public override void Update() {
        base.Update();
        Collidable = isBlock;
        if (!collected && canDash && !isBlock && CollideCheck<Player>()) {
            collected = true;
            Audio.Play("event:/GameHelper/p_switch/p_switch");
            Add(new Coroutine(collectRoutine()));
            if (permanent) SceneAs<Level>().Session.DoNotLoad.Add(id);
        }
    }

    private IEnumerator collectRoutine() {
        for (int i = 0; i < 3; i++) {
            coinSprite.RenderPosition -= 2 * Vector2.UnitY;
            yield return null;
        }
        for (int i = 0; i < 3; i++) {
            coinSprite.RenderPosition -= Vector2.UnitY;
            yield return null;
        }
        yield return null;
        for (int i = 0; i < 3; i++) {
            coinSprite.RenderPosition += Vector2.UnitY;
            yield return null;
        }
        for (int i = 0; i < 3; i++) {
            coinSprite.RenderPosition += 2 * Vector2.UnitY;
            yield return null;
        }
        while (coinSprite.CurrentAnimationFrame is not (3 or 9)) yield return null;
        coinSprite.Play("collect");
        while (coinSprite.Animating) yield return null;
        RemoveSelf();
    }

    public override void Render() {
        Get<LightOcclude>().Visible = isBlock;
        if (isBlock) {
            base.Render();
        } else if (canDash) {
            coinSprite.Render();
        }
    }

    public override void Removed(Scene scene) {
        // no freeze frames
        if (Components != null) {
            foreach (Component component in Components) {
                component.EntityRemoved(scene);
            }
        }
        Scene = null;
    }
}