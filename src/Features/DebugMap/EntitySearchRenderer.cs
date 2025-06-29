using Celeste.Editor;
using Celeste.Mod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public class EntitySearchRenderer(string key, EntitySearch.Mode mode) : Entity {
    private IDictionary<string, List<int[]>> Index => mode switch {
        EntitySearch.Mode.Entities => GameHelper.Session.EntityIndex,
        EntitySearch.Mode.Triggers => GameHelper.Session.TriggerIndex,
        EntitySearch.Mode.Groups => GameHelper.Session.GroupIndex,
        _ => null,
    };

    private readonly string key = key;
    private readonly EntitySearch.Mode mode = mode;

    public override void Render() {
        base.Render();

        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
        // BG blend
        Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * 0.5f);
        Draw.SpriteBatch.End();

        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, MapEditor.Camera.Matrix * Engine.ScreenMatrix);
        foreach(int[] data in Index[key]) {
            if(data[2] > 0 || data[3] > 0 || mode == EntitySearch.Mode.Groups) {
                // sized entity
                Draw.HollowRect(data[0], data[1] - (data[3] == 0 ? 1 : 0), Calc.Max(data[2], 1), Calc.Max(data[3], 1), Color.Cyan);
            } else {
                // sizeless entity
                Draw.HollowRect(data[0] - 1f, data[1] - 2f, 3f, 3f, Color.Cyan);
            }
        }
        Draw.SpriteBatch.End();

        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
        // info headline
        Draw.Rect(0f, 0f, 1920f, 72f, Color.Black);
        ActiveFont.Draw("Showing " + Index[key].Count + " " + key, new Vector2(16f, 4f), Color.Cyan);
        ActiveFont.Draw("F8 to highlight", new Vector2(1904f, 4f), Vector2.UnitX, Vector2.One, Color.Cyan);

        if(MInput.Keyboard.Check(Keys.F8)) {
            foreach(int[] data in Index[key]) {
                // entity ID
                ActiveFont.DrawOutline(
                    key == "spawnpoint" ? "X" : data[4].ToString(),
                    (new Vector2(data[0], data[1]) - MapEditor.Camera.Position + Vector2.UnitX) * MapEditor.Camera.Zoom + new Vector2(960f, 540f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.5f,
                    Color.Cyan,
                    2f,
                    Color.Black
                );
            }
        }
        Draw.SpriteBatch.End();
    }

    public override void Added(Scene scene) {
        scene.Entities.FindFirst<EntitySearchRenderer>()?.RemoveSelf();
        CoreModule.Settings.ShowManualTextOnDebugMap = false;
        base.Added(scene);
    }
}