using Celeste.Editor;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public static class ColorfulDebug {
    private const string CONTROLLER_NAME = "GameHelper/ColorfulDebugController";
    private static readonly Color[] defaultColors = [
        Calc.HexToColor("FF0000"),
        Calc.HexToColor("FFFFFF"),
        Calc.HexToColor("2F4F4F"),
        Calc.HexToColor("000000"),
        Calc.HexToColor("FFB6C1"),
        Calc.HexToColor("00FF00"),
        Calc.HexToColor("FFFF00")
    ];

    private static Dictionary<string, Color[]> Index => GameHelper.Session.DebugColors;

    private static void IndexLevel(Session session) {
        GameHelper.Session.DebugColors = new();
        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int) session.Area.Mode].MapData;
        foreach(LevelData level in mapData.Levels) {
            foreach(EntityData entity in level.Entities) {
                if(entity.Name.Equals(CONTROLLER_NAME)) {
                    string prefix = entity.Attr("roomPrefix");
                    Color[] colorData = [
                        entity.HexColor("spawnColor"),
                        entity.HexColor("fgTileColor"),
                        entity.HexColor("bgTileColor"),
                        entity.HexColor("roomBgColor"),
                        entity.HexColor("berryColor"),
                        entity.HexColor("checkpointColor"),
                        entity.HexColor("jumpthruColor")
                    ];
                    if(prefix.Length == 0) {
                        bool success = Index.TryAdd(level.Name, colorData);
                        if(!success) {
                            Logger.Warn("GameHelper", "Overlapping ColorfulDebugController in room " + level.Name + " (room color already set)");
                        }
                    } else {
                        List<LevelData> affectedLevels = prefix.Equals("*") ?
                            mapData.Levels :
                            mapData.Levels.FindAll(l => l.Name.StartsWith(prefix));
                        affectedLevels.ForEach(l => {
                            bool success = Index.TryAdd(l.Name, colorData);
                            if(!success) {
                                Logger.Warn("GameHelper", "Overlapping ColorfulDebugController in room " + level.Name + " (room " + l.Name + " found for prefix " + prefix + " already has colors set)");
                            }
                        });
                    }
                }
            }
        }
    }

    private static void OnMapEditorRender(On.Celeste.Editor.MapEditor.orig_Render orig, MapEditor self) {
        if(GameHelper.Session.DebugColors == null) {
            IndexLevel(DynamicData.For(self).Get<Session>("CurrentSession"));
        }
        orig(self);
    }

    private static void OnRenderLevel(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig, LevelTemplate self, Camera camera, List<LevelTemplate> allLevels) {
        if(Index.Count > 0) {
            if(!CullHelper.IsRectangleVisible(self.X, self.Y, self.Width, self.Height, 4f, camera)) {
                return;
            }
            if(self.Type == LevelTemplateType.Level) {
                bool vanillaDummy = false;
                if(!Index.TryGetValue(self.Name, out Color[] colors)) {
                    colors = defaultColors;
                    vanillaDummy = self.Dummy;
                }

                bool flash = false;
                // Red blinking if levels are intersecting
                if(Engine.Scene.BetweenInterval(0.1f)) {
                    foreach(LevelTemplate allLevel in allLevels) {
                        if(allLevel != self && allLevel.Rect.Intersects(self.Rect)) {
                            flash = true;
                            break;
                        }
                    }
                }
                Draw.Rect(self.X, self.Y, self.Width, self.Height, (flash ? Color.Red : colors[3]) * 0.5f);
                foreach(Rectangle back in self.backs) {
                    Draw.Rect(self.X + back.X, self.Y + back.Y, back.Width, back.Height, colors[2] * 0.5f);
                }
                foreach(Rectangle solid in self.solids) {
                    Draw.Rect(self.X + solid.X, self.Y + solid.Y, solid.Width, solid.Height, vanillaDummy ? Color.LightGray : colors[1]);
                }
                foreach(Vector2 spawn in self.Spawns) {
                    Draw.Rect((float) self.X + spawn.X, (float) self.Y + spawn.Y - 1f, 1f, 1f, colors[0]);
                }
                foreach(Vector2 strawberry in self.Strawberries) {
                    Draw.HollowRect((float) self.X + strawberry.X - 1f, (float) self.Y + strawberry.Y - 2f, 3f, 3f, colors[4]);
                }
                foreach(Vector2 checkpoint in self.Checkpoints) {
                    Draw.HollowRect((float) self.X + checkpoint.X - 1f, (float) self.Y + checkpoint.Y - 2f, 3f, 3f, colors[5]);
                }
                foreach(Rectangle jumpthru in self.Jumpthrus) {
                    Draw.Rect(self.X + jumpthru.X, self.Y + jumpthru.Y, jumpthru.Width, 1f, colors[6]);
                }
                return;
            }
            Draw.Rect(self.X, self.Y, self.Width, self.Height, Color.LightGray);
            Draw.Rect(self.X + self.Width - self.resizeHoldSize.X, self.Y + self.Height - self.resizeHoldSize.Y, self.resizeHoldSize.X, self.resizeHoldSize.Y, Color.Orange);
        } else {
            orig(self, camera, allLevels);
        }
    }

    public static void Hook() {
        On.Celeste.Editor.MapEditor.Render += OnMapEditorRender;
        On.Celeste.Editor.LevelTemplate.RenderContents += OnRenderLevel;
    }

    public static void Unhook() {
        On.Celeste.Editor.MapEditor.Render -= OnMapEditorRender;
        On.Celeste.Editor.LevelTemplate.RenderContents -= OnRenderLevel;
    }
}