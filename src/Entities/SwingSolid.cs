using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;

namespace Celeste.Mod.GameHelper.Entities;

[CustomEntity("GameHelper/SwingSolid")]
public class SwingSolid : Solid {
    private readonly Vector2 anchor, rope;
    private readonly float radius;
    private readonly List<Image> images = new();
    private bool left = false;
    private float phase = 0f;
    private float maxAng = 0f;
    private float previousX, inputNeutralTime;

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
        radius = rope.Length();
        previousX = X;
        Depth = 1;
    }


    private DashCollisionResults OnDash(Player player, Vector2 direction) {
        if(direction.X != 0f) {
            left = direction.X < 0f;
            maxAng = (float) (Math.PI / 3);
            // brute force valid phase value
            float bestDist = float.MaxValue;
            for(float i = (float) Math.PI / -2f; i < Math.PI / 2f; i += 0.01f) {
                float localDist = Math.Abs(X - getToX(left, maxAng, i));
                if(localDist < bestDist) {
                    phase = i;
                    bestDist = localDist;
                }
            }
            return DashCollisionResults.Rebound;
        }
        return DashCollisionResults.NormalCollision;
    }

    public override void Update() {
        base.Update();

        if(Input.Aim.Value.X == 0f) {
            inputNeutralTime += Engine.DeltaTime;
        } else {
            inputNeutralTime = 0f;
        }

        if(maxAng <= 0f && HasPlayerClimbing() && Math.Sign(Input.Aim.Value.X) != 0) {
            left = Input.Aim.Value.X < 0f;
            maxAng += Engine.DeltaTime / 3f;
            phase = 0;
        }
        if(maxAng > 0f) {
            phase += 2 * Engine.DeltaTime;
            float moveX = getToX(left, maxAng, phase);
            MoveToX(moveX);
            MoveToY(getToY(maxAng, phase));

            bool accelerate = HasPlayerClimbing() && ((inputNeutralTime < 0.5f && Math.Abs(moveX - previousX) < 0.3) || Math.Sign(Input.Aim.Value.X) == Math.Sign(moveX - previousX));
            if(maxAng > 0f && !accelerate)
                maxAng -= Engine.DeltaTime / 6f;
            else if(maxAng < Math.PI / 3f && accelerate) {
                maxAng += Engine.DeltaTime / 6f;
            }
            previousX = moveX;
        } else {
            previousX = X;
        }
    }

    private float getToX(bool left, float maxAng, float phase) {
        float move = (left ? -1 : 1) * (float) (radius * Math.Cos(maxAng * Math.Sin(phase) - Math.PI / 2));
        return anchor.X - Width / 2 + move;
    }

    private float getToY(float maxAng, float phase) {
        float move = (float) (-radius * Math.Sin(maxAng * Math.Sin(phase) - Math.PI / 2));
        return anchor.Y - Height / 2 + move;
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