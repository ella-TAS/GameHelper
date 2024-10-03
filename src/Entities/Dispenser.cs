using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;
using FMOD.Studio;
using System;
using Celeste.Mod.GameHelper.Utils;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Dispenser")]
public class Dispenser : Solid {
    private static EventInstance sound;
    private readonly ParticleType pType;
    private float shootTimer;
    private readonly float maxShootTimer;
    private readonly string flag, arrowSprite;
    private readonly bool facingLeft;

    public Dispenser(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, 16, 16, safe: false) {
        flag = data.Attr("flag");
        facingLeft = data.Bool("faceLeft");
        maxShootTimer = data.Float("cooldown");
        arrowSprite = data.Attr("arrowSprite", "objects/GameHelper/arrow");
        Add(new Image(GFX.Game[data.Attr("sprite", "objects/GameHelper/dispenser")]) {
            FlipX = facingLeft
        });
        Depth = -1;

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
        shootTimer = maxShootTimer;
        SceneAs<Level>().Add(new Arrow(Position + new Vector2(facingLeft ? -16 : 16, 8), facingLeft, arrowSprite));
        if(!Audio.IsPlaying(sound)) {
            sound = Audio.Play("event:/GameHelper/dispenser/dispenser");
        }
        SceneAs<Level>().ParticlesFG.Emit(pType, 50, Position + new Vector2(facingLeft ? 1 : 17, 11), Vector2.UnitY, facingLeft ? MathF.PI : 0);
    }

    public override void Update() {
        base.Update();
        shootTimer -= Engine.DeltaTime;
        if(shootTimer <= 0 && Utils.Util.GetFlag(flag, this)) {
            shoot();
        }
    }
}