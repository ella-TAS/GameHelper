using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/PSwitchBlock")]
public class PSwitchBlock : DashBlock {
    private readonly Sprite coinSprite;
    private readonly string flag;
    private readonly bool startBlock;
    private bool isBlock => startBlock ^ SceneAs<Level>().Session.GetFlag(flag);
    private bool collected;

    public PSwitchBlock(EntityData data, Vector2 levelOffset, EntityID id)
        : base(data.Position + levelOffset, data.Char("tiletype", '3'), data.Width, data.Height, data.Bool("blendIn"), data.Bool("permanent"), data.Bool("canDash"), id) {
        flag = data.Attr("flag");
        startBlock = data.Bool("startAsBlock");
        OnDashCollide = delegate (Player p, Vector2 dir) {
            if(!canDash) return DashCollisionResults.NormalCollision;
            Break(p.Center, dir, true, true);
            return DashCollisionResults.Ignore;
        };
        coinSprite = GameHelper.SpriteBank.Create("mario_coin_" + data.Attr("coinSprite", "blue"));
        Add(coinSprite);
    }

    public override void Update() {
        base.Update();
        Collidable = isBlock;
        if(!collected && canDash && !isBlock && CollideCheck<Player>()) {
            collected = true;
            Add(new Coroutine(collectRoutine()));
        }
    }

    private IEnumerator collectRoutine() {
        for(int i = 0; i < 3; i++) {
            coinSprite.RenderPosition -= 2 * Vector2.UnitY;
            yield return null;
        }
        for(int i = 0; i < 3; i++) {
            coinSprite.RenderPosition -= Vector2.UnitY;
            yield return null;
        }
        yield return null;
        for(int i = 0; i < 3; i++) {
            coinSprite.RenderPosition += Vector2.UnitY;
            yield return null;
        }
        for(int i = 0; i < 3; i++) {
            coinSprite.RenderPosition += 2 * Vector2.UnitY;
            yield return null;
        }
        while(coinSprite.CurrentAnimationFrame != 3 && coinSprite.CurrentAnimationFrame != 9) yield return null;
        coinSprite.Play("collect");
        while(coinSprite.Animating) yield return null;
        RemoveAndFlagAsGone();
    }

    public override void Render() {
        if(isBlock && !collected) {
            base.Render();
        } else if(canDash) {
            coinSprite.Render();
        }
    }

    public override void Removed(Scene scene) { }
}