using Celeste.Editor;
using Celeste.Mod.GameHelper.Utils.Exceptions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public static class EntitySearch {
    private class EntityTagComparer : IComparer<string> {
        public int Compare(string x, string y) {
            bool xMod = x.Contains('/');
            bool yMod = y.Contains('/');
            if (xMod == yMod) {
                return StringComparer.CurrentCultureIgnoreCase.Compare(x, y);
            }
            return xMod ? 1 : -1;
        }
    }

    private static SortedDictionary<string, List<int[]>> EntityIndex => GameHelper.Session.EntityIndex;
    private static SortedDictionary<string, List<int[]>> TriggerIndex => GameHelper.Session.TriggerIndex;
    private static SortedDictionary<string, List<int[]>> GroupIndex => GameHelper.Session.GroupIndex;

    public enum Mode { Entities, Triggers, Groups }

    private static void IndexLevel(Session session) {
        GameHelper.Session.EntityIndex = new(new EntityTagComparer());
        GameHelper.Session.TriggerIndex = new(new EntityTagComparer());
        GameHelper.Session.GroupIndex = new(StringComparer.CurrentCultureIgnoreCase);
        GameHelper.Session.RecentSearch = new();
        GameHelper.Session.SearchSortCount = true;
        GameHelper.Session.SearchMode = Mode.Entities;

        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int) session.Area.Mode].MapData;
        foreach (LevelData level in mapData.Levels) {
            // entities
            foreach (EntityData entity in level.Entities) {
                if (!EntitySearchData.SpecificOffset.TryGetValue(entity.Name, out int[] offset)) {
                    offset = [0, 0];
                }

                int[] data = [
                    (int) ((entity.Position.X + level.Bounds.X) / 8f) + offset[0],
                    (int) ((entity.Position.Y + level.Bounds.Y) / 8f) + offset[1],
                    (int) (entity.Width / 8f),
                    (int) (entity.Height / 8f),
                    entity.ID
                ];
                if (!EntityIndex.TryGetValue(entity.Name, out List<int[]> list)) {
                    list = new List<int[]>();
                    EntityIndex.Add(entity.Name, list);
                }
                list.Add(data);

                // groups
                foreach (KeyValuePair<string, string[]> group in EntitySearchData.Groups) {
                    if (group.Value.Contains(entity.Name)
                        || group.Key.Equals("AllEntities")
                        || (group.Key.Equals("VanillaEntities") && !entity.Name.Contains('/'))
                        || (group.Key.Equals("ModdedEntities") && entity.Name.Contains('/'))
                    ) {
                        if (!GroupIndex.TryGetValue(group.Key, out List<int[]> list2)) {
                            list2 = new List<int[]>();
                            GroupIndex.Add(group.Key, list2);
                        }
                        list2.Add(data);
                    }
                }
            }

            // spawnpoints
            foreach (Vector2 spawn in level.Spawns) {
                if (!EntityIndex.TryGetValue("spawnpoint", out List<int[]> list)) {
                    list = new List<int[]>();
                    EntityIndex.Add("spawnpoint", list);
                }
                list.Add([(int) (spawn.X / 8f), (int) (spawn.Y / 8f), 0, 0, 0]);
            }

            // triggers
            foreach (EntityData trigger in level.Triggers) {
                if (!TriggerIndex.TryGetValue(trigger.Name, out List<int[]> list)) {
                    list = new List<int[]>();
                    TriggerIndex.Add(trigger.Name, list);
                }
                int[] data = [
                    (int) ((trigger.Position.X + level.Bounds.X) / 8f),
                    (int) ((trigger.Position.Y + level.Bounds.Y) / 8f),
                    (int) (trigger.Width / 8f),
                    (int) (trigger.Height / 8f),
                    trigger.ID
                ];
                list.Add(data);

                // groups
                foreach (KeyValuePair<string, string[]> group in EntitySearchData.Groups) {
                    if (group.Value.Contains(trigger.Name) || group.Key.Equals("AllTriggers")) {
                        if (!GroupIndex.TryGetValue(group.Key, out List<int[]> list2)) {
                            list2 = new List<int[]>();
                            GroupIndex.Add(group.Key, list2);
                        }
                        list2.Add(data);
                    }
                }
            }
        }

        if (!GroupIndex.ContainsKey("ModdedEntities")) {
            // AllEntities and VanillaEntities are the same
            GroupIndex.Remove("VanillaEntities");
        }
    }

    private static void OnMapEditorUpdate(On.Celeste.Editor.MapEditor.orig_Update orig, MapEditor self) {
        EntitySearchUI ui = self.Entities.FindFirst<EntitySearchUI>();
        if (ui != null) {
            ui.Update();
            return;
        }

        orig(self);
        if (MInput.Keyboard.Pressed(Keys.F7)) {
            if (EntityIndex == null) {
                IndexLevel(DynamicData.For(self).Get<Session>("CurrentSession"));
            }
            self.Add(new EntitySearchUI());
        }
    }

    private static void OnMapEditorRender(On.Celeste.Editor.MapEditor.orig_Render orig, MapEditor self) {
        orig(self);
        self.Entities.FindFirst<EntitySearchRenderer>()?.Render();
        self.Entities.FindFirst<EntitySearchUI>()?.Render();
    }

    private static void OnUpdateMouse(On.Celeste.Editor.MapEditor.orig_UpdateMouse orig, MapEditor self) {
        if (self.Entities.FindFirst<EntitySearchUI>() == null) {
            orig(self);
        }
    }

    private static void OnRenderManualText(On.Celeste.Editor.MapEditor.orig_RenderManualText orig, MapEditor self) {
        if (self.Entities.FindFirst<EntitySearchUI>() == null) {
            orig(self);
        }
    }

    private static void ILRenderManualText(ILContext context) {
        ILCursor cursor = new(context);
        if (cursor.TryGotoNext(
            ins => ins.MatchLdstr((string) typeof(MapEditor).GetField("ManualText", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))
        )) {
            cursor.Index++;
            cursor.EmitDelegate(EmitManualText);
        } else {
            throw new ILException("MapEditor.RenderManualText");
        }
    }

    private static string EmitManualText(string previous) {
        return "F7:           Entity Search (Game Helper)\n"
            + "F8:           Entity Search: Show IDs\n\n"
            + previous;
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