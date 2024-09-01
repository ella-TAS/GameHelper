using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Entities.MousePuzzle;

[Tracked]
[CustomEntity("GameHelper/MouseHole")]
public class MouseHole : Solid {
    private readonly Sprite sprite;
    private bool wasFlag, complete;
    private float spawnTimer;
    private readonly bool spawner, resetFlagOnDeath;
    private readonly string flag;

    public MouseHole(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: true) {
        spawner = data.Bool("spawner");
        flag = data.Attr("flag");
        resetFlagOnDeath = data.Bool("resetFlagOnDeath");
        Depth = -1;
        Collider = new Hitbox(16, 16);
        Add(sprite = GameHelper.SpriteBank.Create("mouse_hole"));
        if(!spawner) {
            sprite.Play("exit");
        }
    }

    public override void Update() {
        base.Update();
        if(spawner) {
            spawnTimer += Engine.DeltaTime;
            bool isFlag = SceneAs<Level>().Session.GetFlag(flag);
            if(isFlag && !wasFlag) {
                sprite.Play("opening");
                wasFlag = true;
                spawnTimer = 0.25f;
            }
            if(!isFlag && wasFlag) {
                sprite.Play("closing");
                wasFlag = false;
            }
            if(isFlag && spawnTimer >= 0.25f) {
                SceneAs<Level>().Add(new Mouse(Position));
                spawnTimer = 0;
            }
        }
    }

    public bool Complete() {
        if(!spawner && !complete) {
            SceneAs<Level>().Session.SetFlag(flag);
            sprite.Play("complete");
            complete = true;
        }
        return !spawner;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if(!spawner && SceneAs<Level>().Session.GetFlag(flag)) {
            complete = true;
            sprite.Play("complete");
        }
    }

    public override void Removed(Scene scene) {
        if(resetFlagOnDeath) {
            SceneAs<Level>().Session.SetFlag(flag, false);
        }
        base.Removed(scene);
    }
}