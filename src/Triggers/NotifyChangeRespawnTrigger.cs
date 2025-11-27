using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/NotifyChangeRespawnTrigger")]

public class NotifyChangeRespawnTrigger : ChangeRespawnTrigger {
    private const string TERRARIA_SID = "CrossoverCollab/1-Submissions/Terraria";
    private const string DEFAULT_SFX = "event:/GameHelper/respawn/Respawn";

    private readonly ParticleType pType;
    private readonly string flag, sfxPath;
    private readonly bool invertFlag, notifySound, notifyVisual;
    private bool atTrigger;

    public NotifyChangeRespawnTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        flag = data.Attr("flag");
        invertFlag = data.Bool("invertFlag");
        notifySound = data.Bool("notifySound", true);
        notifyVisual = data.Bool("notifyVisual", true);
        atTrigger = data.Bool("atTrigger", true);
        sfxPath = data.Attr("sfxPath");

        if (sfxPath.Length == 0) {
            sfxPath = DEFAULT_SFX;
        }

        //particles
        pType = new() {
            Color = new Color(0, 255, 0),
            Color2 = new Color(0, 180, 0),
            ColorMode = ParticleType.ColorModes.Choose,
            DirectionRange = (float) Math.PI * 2f,
            LifeMin = 0.2f,
            LifeMax = 0.5f,
            SpeedMin = 5,
            SpeedMax = 40,
            Friction = 20f,
            Size = 1
        };
    }

    public override void OnEnter(Player player) {
        Session session = SceneAs<Level>()?.Session;
        if (session == null || !Util.GetFlag(flag, SceneAs<Level>(), true, invertFlag)) {
            return;
        }

        if (SolidCheck() && (!session.RespawnPoint.HasValue || session.RespawnPoint.Value != Target)) {
            if (notifySound) {
                Audio.Play(sfxPath);
            }
            if (notifyVisual) {
                SceneAs<Level>().ParticlesFG.Emit(pType, 100, atTrigger ? Center : player.Center, Vector2.One * 2f);
            }
        }

        base.OnEnter(player);
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (SceneAs<Level>().Session.Area.SID.Equals(TERRARIA_SID)) {
            atTrigger = true;
        }
    }
}