// heavily inspired by KoseiHelper

using Celeste.Editor;
using Celeste.Mod.GameHelper.Entities.Controllers;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public struct DebugDecalData {
    public string type;
    public Vector2 position;
    public int width;
    public int height;
    public bool hollow;
    public float thickness;
    public string data;
    public float scaleX;
    public float scaleY;
    public Color color;
}

public static class ColorfulDebug {
    private const string CONTROLLER_COLOR = "GameHelper/ColorfulDebugController";
    private const string CONTROLLER_DECAL = "GameHelper/DebugDecalController";

    public const string TYPE_IMAGE = "Image";
    public const string TYPE_RECTANGLE = "Rectangle";
    public const string TYPE_LINE = "Line";
    public const string TYPE_TEXT = "Text";

    private static readonly Color[] defaultColors = [
        Calc.HexToColor("FF0000"),
        Calc.HexToColor("FFFFFF"),
        Calc.HexToColor("2F4F4F"),
        Calc.HexToColor("000000"),
        Calc.HexToColor("FFB6C1"),
        Calc.HexToColor("00FF00"),
        Calc.HexToColor("FFFF00")
    ];

    private static Dictionary<string, Color[]> ColorIndex => GameHelper.Session.DebugColors;
    private static Dictionary<string, List<DebugDecalData>> DecalIndex => GameHelper.Session.DebugDecals;

    private static void IndexLevel(Session session) {
        GameHelper.Session.DebugColors = new();
        GameHelper.Session.DebugDecals = new();

        bool success;
        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int) session.Area.Mode].MapData;
        foreach(LevelData level in mapData.Levels) {
            foreach(EntityData entity in level.Entities) {
                switch(entity.Name) {

                    case CONTROLLER_COLOR:
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
                            success = ColorIndex.TryAdd(level.Name, colorData);
                            if(!success) {
                                Logger.Warn("GameHelper", $"Overlapping ColorfulDebugController in room {level.Name} (room color already set)");
                            }
                        } else {
                            List<LevelData> affectedLevels = prefix.Equals("*") ?
                                mapData.Levels :
                                mapData.Levels.FindAll(l => l.Name.StartsWith(prefix));
                            affectedLevels.ForEach(l => {
                                bool success = ColorIndex.TryAdd(l.Name, colorData);
                                if(!success) {
                                    Logger.Warn("GameHelper", $"Overlapping ColorfulDebugController in room {level.Name} (room {l.Name} found for prefix {prefix} already has colors set)");
                                }
                            });
                        }
                        break;

                    case CONTROLLER_DECAL:
                        if(!DecalIndex.TryGetValue(level.Name, out List<DebugDecalData> list)) {
                            list = new();
                            DecalIndex.Add(level.Name, list);
                        }
                        int width = entity.Width;
                        int height = entity.Height;
                        if(entity.Nodes.Length > 0) {
                            Vector2 node = entity.Nodes[0] - entity.Position;
                            width = (int) node.X;
                            height = (int) node.Y;
                        }
                        list.Add(new() {
                            type = entity.Attr("type"),
                            position = entity.Position / 8f,
                            width = width / 8,
                            height = height / 8,
                            hollow = entity.Bool("hollow"),
                            thickness = entity.Float("thickness", entity.Float("scale")),
                            data = entity.Attr("dialog", entity.Attr("sprite")),
                            scaleX = entity.Float("scaleX"),
                            scaleY = entity.Float("scaleY"),
                            color = entity.HexColor("color")
                        });
                        break;
                }
            }
        }
    }

    private static void OnMapEditorRender(On.Celeste.Editor.MapEditor.orig_Render orig, MapEditor self) {
        if(ColorIndex == null || DecalIndex == null) {
            IndexLevel(DynamicData.For(self).Get<Session>("CurrentSession"));
        }
        orig(self);
    }

    private static void OnRenderLevel(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig, LevelTemplate self, Camera camera, List<LevelTemplate> allLevels) {
        if(ColorIndex.Count == 0 && DecalIndex.Count == 0) {
            orig(self, camera, allLevels);
            return;
        }

        if(!CullHelper.IsRectangleVisible(self.X, self.Y, self.Width, self.Height, 4f, camera)) {
            return;
        }

        if(self.Type == LevelTemplateType.Level) {
            bool vanillaDummy = false;
            if(!ColorIndex.TryGetValue(self.Name, out Color[] colors)) {
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
                Draw.Rect(self.X + spawn.X, self.Y + spawn.Y - 1f, 1f, 1f, colors[0]);
            }
            foreach(Vector2 strawberry in self.Strawberries) {
                Draw.HollowRect(self.X + strawberry.X - 1f, self.Y + strawberry.Y - 2f, 3f, 3f, colors[4]);
            }
            foreach(Vector2 checkpoint in self.Checkpoints) {
                Draw.HollowRect(self.X + checkpoint.X - 1f, self.Y + checkpoint.Y - 2f, 3f, 3f, colors[5]);
            }
            foreach(Rectangle jumpthru in self.Jumpthrus) {
                Draw.Rect(self.X + jumpthru.X, self.Y + jumpthru.Y, jumpthru.Width, 1f, colors[6]);
            }

            if(DecalIndex.TryGetValue(self.Name, out List<DebugDecalData> decals)) {
                foreach(DebugDecalData d in decals) {
                    Vector2 offset = new(self.X, self.Y);
                    switch(d.type) {
                        case TYPE_IMAGE:
                            Image image = new(GFX.Game[d.data]) {
                                Color = d.color,
                                Scale = new(d.scaleX / 8f, d.scaleY / 8f)
                            };
                            image.Position = new(d.position.X + self.X - (image.Width * d.scaleX / 16f), d.position.Y + self.Y - (image.Height * d.scaleY / 16f));
                            image.Render();
                            break;

                        case TYPE_RECTANGLE:
                            if(d.hollow) {
                                Draw.HollowRect(d.position.X + self.X, d.position.Y + self.Y, d.width, d.height, d.color);
                            } else {
                                Draw.Rect(d.position.X + self.X, d.position.Y + self.Y, d.width, d.height, d.color);
                            }
                            break;

                        case TYPE_LINE:
                            Draw.Line(d.position + offset, d.position + new Vector2(d.width, d.height) + offset, d.color, d.thickness);
                            break;

                        case TYPE_TEXT:
                            ActiveFont.Draw(Dialog.Clean(d.data), d.position + new Vector2(self.X, self.Y), 0.5f * Vector2.One, Vector2.One * d.thickness / 10f, d.color);
                            break;
                    }
                }
            }
        } else {
            Draw.Rect(self.X, self.Y, self.Width, self.Height, Color.LightGray);
            Draw.Rect(self.X + self.Width - self.resizeHoldSize.X, self.Y + self.Height - self.resizeHoldSize.Y, self.resizeHoldSize.X, self.resizeHoldSize.Y, Color.Orange);
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