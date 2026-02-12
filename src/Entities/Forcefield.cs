using Celeste.Mod.Entities;
using Celeste.Mod.GameHelper.Utils;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/Forcefield")]
public class Forcefield : Entity {
    public TimeRateModifier timeRateModifier;
    private int width, height;
    private Direction dir;
    private bool horiz;
    private float force;
    private Color color;
    private bool addKeyBlock;

    public Forcefield(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset) {
        if (!Enum.TryParse(data.Attr("direction"), out dir)) {
            throw new ArgumentException($"{data.Attr("direction")} is not a valid Forcefield direction");
        }
        horiz = dir.isHorizontal();

        addKeyBlock = horiz && data.Bool("speedCheck");
        Depth = 2;
        width = data.Width;
        height = data.Height;
        color = data.HexColor("color");
        force = data.Float("force");
        if (force < 0) {
            force *= -1;
            dir = dir.Mirror();
        }

        Collider = new Hitbox(width, height);
        Add(new PlayerCollider(onPlayer));
        Add(timeRateModifier = new TimeRateModifier(1f));
    }

    private void onPlayer(Player player) {
        if (player.Get<ForcefieldComponent>() is not ForcefieldComponent component) {
            // first touch
            component = new ForcefieldComponent(this, horiz ? player.Speed.X : 0f, force * dir.ToVector());
            player.Add(component);
        }

        component.ResetTimer();
        player.StateMachine.State = Player.StNormal;
        player.dashCooldownTimer = 0.02f;
        player.dashAttackTimer = 0.02f;
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        if (addKeyBlock) {
            float keyPosX = dir == Direction.Right ? X - 16 : X + width;
            SceneAs<Level>().Add(new ForcefieldSpeedBlock(new Vector2(keyPosX, Y), height));
        }
    }

    public override void Render() {
        base.Render();
        Draw.Rect(Collider, color * 0.15f);
        Draw.HollowRect(Collider, color * 0.7f);
    }
}