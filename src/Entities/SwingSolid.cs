using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {
    private readonly Vector2 anchor, rope;
    private readonly bool stickOnDash;
    private readonly float radius;
    private readonly float constSwingAngle = 0; // (float) (Math.PI / 8f);
    private readonly List<Image> images = new();
    private bool left = false;
    private float phase = 0f;
    private float maxAng = 0f;

    public SwingSolid(EntityData data, Vector2 levelOffset) : base(data.Position + levelOffset, data.Width, data.Height, safe: false) {
        anchor = data.Nodes[0] + levelOffset + Vector2.One * 24;
        rope = (Center - anchor).Length() * Vector2.UnitY;
        Add(new Image(GFX.Game[data.Attr("sprite", "objects/GameHelper/swing/swing_solid")]) {
            RenderPosition = Vector2.One * -5
        });

        for(int i = 0; i < (int) ((anchor - Center).Length() / 4); i++) {
            images.Add(new Image(GFX.Game[data.Attr("chainSpritePrefix", "objects/GameHelper/swing/chain0") + GameHelper.Random.Range(1, 5)]));
        }

        OnDashCollide = OnDash;
        stickOnDash = false;
        OnDashCollide = OnDash;
        radius = rope.Length();
        Depth = 1;
    }


    private DashCollisionResults OnDash(Player player, Vector2 direction) {
        if(maxAng <= 0f && direction.X != 0f && constSwingAngle == 0f) {
            left = direction.X < 0f;
            maxAng = (float) (Math.PI / 3);
            phase = 0;
            if(!stickOnDash) return DashCollisionResults.Rebound;
        }
        return DashCollisionResults.NormalCollision;
    }

    public override void Update() {
        base.Update();

        if(maxAng <= 0f && HasPlayerClimbing() && Math.Sign(Input.Aim.value.X) != 0) {
            left = Input.Aim.value.X < 0f;
            maxAng += Engine.DeltaTime / 6f;
        }
        if(maxAng > 0f || constSwingAngle != 0f) {
            phase += 2 * Engine.DeltaTime;
            float moveToX = (left ? -1 : 1) * (float) (radius * Math.Cos((maxAng + constSwingAngle) * Math.Sin(phase) - Math.PI / 2));
            float moveToY = (float) (-radius * Math.Sin((maxAng + constSwingAngle) * Math.Sin(phase) - Math.PI / 2));
            MoveToX(anchor.X - Width / 2 + moveToX);
            MoveToY(anchor.Y - Height / 2 + moveToY);

            bool keepMomentum = HasPlayerClimbing() && Math.Sign(Input.Aim.Value.X) == Math.Sign(moveToX);
            if(maxAng > 0f && !keepMomentum)
                maxAng -= Engine.DeltaTime / 6f;
            else if(maxAng < Math.PI / 3f && keepMomentum) {
                maxAng += Engine.DeltaTime / 6f;
            }
        }
    }

    public override void Added(Scene scene) {
        base.Added(scene);
        Center = anchor + rope;
    }

    public override void Render() {
        Vector2 unit = 8f * Vector2.UnitY;
        float rotation = (anchor - Center + unit).Angle() + (float) (Math.PI / 2);
        IEnumerator<Image> im = images.GetEnumerator();
        for(Vector2 pos = anchor; pos != Center + unit; pos = Calc.Approach(pos, Center + unit, 8f)) {
            im.MoveNext();
            if(im.Current == null) break;
            im.Current.SetOrigin(8f, 0f);
            im.Current.Position = pos - unit;
            im.Current.Rotation = rotation;
            im.Current.Render();
        }
        base.Render();
    }
}