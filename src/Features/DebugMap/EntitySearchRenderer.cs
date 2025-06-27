using Celeste.Editor;
using Celeste.Mod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public class EntitySearchRenderer(string key) : Entity {
    private static SortedDictionary<string, List<Vector2>> Index => GameHelper.Session.EntitySearchIndex;
    private readonly string key = key;

    public override void Render() {
        base.Render();

        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.5f);
        Draw.SpriteBatch.End();

        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, MapEditor.Camera.Matrix * Engine.ScreenMatrix);
        foreach(Vector2 pos in Index[key]) {
            Draw.HollowRect(pos.X - 1f, pos.Y - 2f, 3f, 3f, Color.Cyan);
        }
        Draw.SpriteBatch.End();
    }

    public override void Added(Scene scene) {
        scene.Entities.FindFirst<EntitySearchRenderer>()?.RemoveSelf();
        CoreModule.Settings.ShowManualTextOnDebugMap = false;
        base.Added(scene);
    }
}