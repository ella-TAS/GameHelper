using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/FireTrap")]
public class FireTrap : Entity {
    private readonly ParticleType fireParticles;
    private bool active;
    private readonly float delay;

    public FireTrap(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        delay = data.Float("delay");
        Collider = new Hitbox(8, 64, 0, -56);
        Depth = -1;
        Add(new PlayerCollider(onPlayer));
        Add(new Image(GFX.Game[data.Attr("sprite", "objects/GameHelper/fire_trap")]) {
            RenderPosition = new Vector2(-4, -8)
        });

        //particles
        fireParticles = new ParticleType() {
            Color = Color.Orange,
            Color2 = Color.OrangeRed,
            ColorMode = ParticleType.ColorModes.Choose,
            DirectionRange = 0.3f,
            LifeMin = 0.4f,
            LifeMax = 0.9f,
            SpeedMin = 60,
            SpeedMax = 120,
            Acceleration = Vector2.UnitY * 0.1f,
            Friction = 10f,
            FadeMode = ParticleType.FadeModes.Late
        };
    }

    private void onPlayer(Player p) {
        if (active) {
            p.Die(Vector2.Zero);
        } else {
            active = true;
            Add(new Coroutine(routineActivation()));
            Collider = new Hitbox(8, 6, 0, 2);
        }
    }

    public override void Update() {
        base.Update();
        if (active) {
            Collider.Height = Calc.Approach(Collider.Height, 64f, 1.5f);
            Collider.Position.Y = Calc.Approach(Collider.Position.Y, -56f, 1.5f);
            SceneAs<Level>().ParticlesFG.Emit(fireParticles, 2, Position + new Vector2(5, 3), Vector2.UnitX * 2f, 4.7123890f);
        }
    }

    private IEnumerator routineActivation() {
        Audio.Play("event:/GameHelper/firetrap/firetrap");
        Collidable = false;
        yield return delay;
        Collidable = true;
        yield return 1f;
        active = false;
    }
}