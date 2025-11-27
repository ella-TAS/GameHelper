using Celeste.Mod.GameHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Utils;

public class SwingSolidVine : Entity {
    private readonly SwingSolid block;
    private readonly List<Image> images = new();

    public SwingSolidVine(SwingSolid block) {
        this.block = block;
        Depth = 2;
    }

    public override void Added(Scene scene) {
        base.Added(scene);

        for (int i = 0; i < (int) ((block.anchor - block.Center).Length() / 4); i++) {
            images.Add(new Image(GFX.Game[block.vinePath + GameHelper.Random.Range(1, 5)]));
        }
    }

    public override void Render() {
        base.Render();
        Vector2 unit = 8f * Vector2.UnitY;
        float rotation = (block.anchor - block.Center + unit).Angle() + (float) (Math.PI / 2);
        IEnumerator<Image> im = images.GetEnumerator();
        for (Vector2 pos = block.anchor; pos != block.Center + unit; pos = Calc.Approach(pos, block.Center + unit, 8f)) {
            im.MoveNext();
            if (im.Current == null) break;
            im.Current.SetOrigin(8f, 0f);
            im.Current.Position = pos - unit;
            im.Current.Rotation = rotation;
            im.Current.Render();
        }
    }
}