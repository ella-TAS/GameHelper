using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Dispenser")]
public class Dispenser : Solid {
    private ParticleType pType;
    private bool wasFlag;
    private string flag;
    private bool facingLeft;

    public Dispenser(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, 16, 16, safe: false) {
        flag = data.Attr("flag");
        facingLeft = data.Bool("faceLeft");
        base.Depth = -1;
        Sprite sprite = GameHelper.SpriteBank.Create("dispenser");
        sprite.FlipX = facingLeft;
        Add(sprite);

        //particles
        pType = new ParticleType() {
            Color = new Color(67, 67, 67), //light gray
            Color2 = new Color(43, 43, 43), //dark gray
            ColorMode = ParticleType.ColorModes.Choose,
            DirectionRange = 0.2f,
            LifeMax = 0.7f,
            LifeMin = 0.2f,
            SpeedMin = 0,
            SpeedMax = 50,
            Acceleration = 0.2f * Vector2.UnitY,
            Friction = 0.7f,
            Size = 1
        };
    }

    private void shoot() {
        SceneAs<Level>().Add(new Arrow(Position + new Vector2(facingLeft ? -16 : 16, 8), facingLeft));
        Audio.Play("event:/GameHelper/dispenser/dispenser");
        SceneAs<Level>().ParticlesFG.Emit(pType, 50, Position + new Vector2(facingLeft ? 1 : 17, 11), Vector2.UnitY, facingLeft ? 3.1415927f : 0);
    }

    public override void Update() {
        base.Update();
        bool isFlag = SceneAs<Level>().Session.GetFlag(flag);
        if(!wasFlag && isFlag) {
            wasFlag = true;
            shoot();
        } else if(wasFlag && !isFlag) {
            wasFlag = false;
        }
    }
}

public class Arrow : Actor {
    private bool facingLeft;

    public Arrow(Vector2 position, bool facingLeft) : base(position) {
        this.facingLeft = facingLeft;
        base.Collider = new Hitbox(16, 4);
        base.Depth = -1;
        Sprite sprite = GameHelper.SpriteBank.Create("arrow");
        sprite.FlipX = facingLeft;
        Add(sprite);
        Add(new PlayerCollider(onCollide));
    }

    public override void Update() {
        base.Update();
        if(!Collidable) {
            return;
        }
        bool collided = MoveH((facingLeft ? -240 : 240) * Engine.DeltaTime);
        if(collided) {
            Collidable = false;
            Position.X += (facingLeft ? -2 : 2);
            Add(new Coroutine(routineDespawn()));
        }
    }

    private void onCollide(Player p) {
        p.Die(Vector2.UnitX * (facingLeft ? -1 : 1));
    }

    private IEnumerator routineDespawn() {
        yield return 10f;
        RemoveSelf();
    }
}