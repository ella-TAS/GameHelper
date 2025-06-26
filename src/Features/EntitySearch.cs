using Celeste.Editor;
using Celeste.Mod.GameHelper.Utils.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.GameHelper.Features;

public static class EntitySearch {
    private static SortedDictionary<string, List<Vector2>> Index => GameHelper.Session.EntitySearchIndex;

    private static void IndexLevel(Session session) {
        GameHelper.Session.EntitySearchIndex = new(StringComparer.CurrentCultureIgnoreCase);

        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int) session.Area.Mode].MapData;
        foreach(LevelData level in mapData.Levels) {
            foreach(EntityData entity in level.Entities) {
                if(!Index.TryGetValue(entity.Name, out List<Vector2> list)) {
                    list = new();
                    Index.Add(entity.Name, list);
                }
                list.Add(entity.Position + level.Position);
            }
        }
    }

    private static void OnMapEditorUpdate(On.Celeste.Editor.MapEditor.orig_Update orig, MapEditor self) {
        EntitySearchUI ui = self.Entities.FindFirst<EntitySearchUI>();
        if(ui != null) {
            ui.Update();
            return;
        }

        orig(self);
        if(MInput.Keyboard.Pressed(Keys.F8)) {
            if(Index == null) {
                IndexLevel(DynamicData.For(self).Get<Session>("CurrentSession"));
            }
            self.Add(new EntitySearchUI());
        }
    }

    private static void OnMapEditorRender(On.Celeste.Editor.MapEditor.orig_Render orig, MapEditor self) {
        orig(self);
        self.Entities.FindFirst<EntitySearchUI>()?.Render();
    }

    private static void OnUpdateMouse(On.Celeste.Editor.MapEditor.orig_UpdateMouse orig, MapEditor self) {
        if(self.Entities.FindFirst<EntitySearchUI>() == null) {
            orig(self);
        }
    }

    private static void OnRenderManualText(On.Celeste.Editor.MapEditor.orig_RenderManualText orig, MapEditor self) {
        if(self.Entities.FindFirst<EntitySearchUI>() == null) {
            orig(self);
        }
    }

    private static void ILRenderManualText(ILContext context) {
        ILCursor cursor = new(context);
        if(cursor.TryGotoNext(
            ins => ins.MatchLdstr((string) typeof(MapEditor).GetField("ManualText", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))
        )) {
            cursor.Index++;
            cursor.EmitDelegate(EmitManualText);
        } else {
            throw new ILException("MapEditor.RenderManualText");
        }
    }

    private static string EmitManualText(string previous) {
        return "F8:           Entity Search (Game Helper)\n\n" + previous;
    }

    public static void Hook() {
        On.Celeste.Editor.MapEditor.Update += OnMapEditorUpdate;
        On.Celeste.Editor.MapEditor.Render += OnMapEditorRender;
        On.Celeste.Editor.MapEditor.UpdateMouse += OnUpdateMouse;
        On.Celeste.Editor.MapEditor.RenderManualText += OnRenderManualText;
        IL.Celeste.Editor.MapEditor.RenderManualText += ILRenderManualText;
    }

    public static void Unhook() {
        On.Celeste.Editor.MapEditor.Update -= OnMapEditorUpdate;
        On.Celeste.Editor.MapEditor.Render -= OnMapEditorRender;
        On.Celeste.Editor.MapEditor.UpdateMouse -= OnUpdateMouse;
        On.Celeste.Editor.MapEditor.RenderManualText -= OnRenderManualText;
        IL.Celeste.Editor.MapEditor.RenderManualText -= ILRenderManualText;
    }
}