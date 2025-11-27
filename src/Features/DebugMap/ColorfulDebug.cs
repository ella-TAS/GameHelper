// inspired by KoseiHelper

using Celeste.Editor;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public static partial class ColorfulDebug {
    internal static event Func<Session, LevelData, EntityData, Dictionary<string, object>> GenerateDebugDecalEvent;

    private const string CONTROLLER_COLOR = "GameHelper/ColorfulDebugController";
    private const string CONTROLLER_DECAL = "GameHelper/DebugDecalController";
    private const string CONTROLLER_TILES = "GameHelper/TileDebugDecalConverter";

    public const string TYPE_IMAGE = "Image";
    public const string TYPE_ANIMATION = "Animation";
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

    private static int generatedDecals;

    private static void IndexLevel(Session session) {
        GameHelper.Session.DebugColors ??= new();
        GameHelper.Session.DebugDecals ??= new();

        generatedDecals = 0;
        bool success;
        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int) session.Area.Mode].MapData;
        foreach (LevelData level in mapData.Levels) {
            foreach (EntityData entity in level.Entities) {
                InvokeGeneratorHooks(session, level, entity);

                switch (entity.Name) {
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
                    if (prefix.Length == 0) {
                        success = ColorIndex.TryAdd(level.Name, colorData);
                        if (!success) {
                            Logger.Warn("GameHelper", $"Overlapping ColorfulDebugController in room {level.Name} (room color already set)");
                        }
                    } else {
                        List<LevelData> affectedLevels = prefix.Equals("*") ?
                            mapData.Levels :
                            mapData.Levels.FindAll(l => l.Name.StartsWith(prefix));
                        affectedLevels.ForEach(l => {
                            bool success = ColorIndex.TryAdd(l.Name, colorData);
                            if (!success) {
                                Logger.Warn("GameHelper", $"Overlapping ColorfulDebugController in room {level.Name} (room {l.Name} found for prefix {prefix} already has colors set)");
                            }
                        });
                    }
                    break;

                case CONTROLLER_DECAL:
                    if (!DecalIndex.TryGetValue(level.Name, out List<DebugDecalData> list)) {
                        list = new();
                        DecalIndex.Add(level.Name, list);
                    }
                    int width = entity.Width;
                    int height = entity.Height;
                    if (entity.Nodes.Length > 0) {
                        Vector2 node = entity.Nodes[0] - entity.Position;
                        width = (int) node.X;
                        height = (int) node.Y;
                    }
                    string type = entity.Attr("type");
                    string data = entity.Attr("dialog", entity.Attr("sprite"));
                    bool useGui = entity.Bool("useGui");
                    List<string> textures = null;
                    if (type.Equals(TYPE_ANIMATION)) {
                        string basePath = entity.Attr("spriteName");
                        Regex regex = new("^" + basePath.Replace("/", @"\/") + @"\d+$");
                        Atlas atlas = useGui ? GFX.Gui : GFX.Game;
                        textures = [.. atlas.Textures.Keys.Where(k => regex.IsMatch(k))];
                        if (textures.Count == 0) {
                            Logger.Warn("GameHelper", $"Animated Debug Decal Controller in room {level.Name} found no matching images");
                            break;
                        }
                    }
                    list.Add(new() {
                        type = type,
                        position = entity.Position / 8f,
                        width = width / 8,
                        height = height / 8,
                        hollow = entity.Bool("hollow"),
                        thickness = entity.Float("thickness", entity.Float("scale")),
                        data = data,
                        scaleX = entity.Float("scaleX"),
                        scaleY = entity.Float("scaleY"),
                        color = entity.HexColor("color"),
                        animationSpeed = Calc.Max(entity.Float("animationSpeed"), 0.001f),
                        textures = textures,
                        useGui = useGui,
                        rotation = entity.Float("rotation")
                    });
                    break;

                case CONTROLLER_TILES:
                    if (!DecalIndex.TryGetValue(level.Name, out list)) {
                        list = new();
                        DecalIndex.Add(level.Name, list);
                    }
                    bool fg = entity.Bool("fg");
                    char match = entity.Char("tileset");
                    Color color = entity.HexColor("color");
                    Regex tileRegex = TileRegex();
                    string[] tileRows = tileRegex.Split(fg ? level.Solids : level.Bg);
                    for (int i = 0; i < tileRows.Length; i++) {
                        for (int j = 0; j < tileRows[i].Length; j++) {
                            if (tileRows[i][j] == match) {
                                list.Add(new() {
                                    type = TYPE_RECTANGLE,
                                    position = new Vector2(j, i),
                                    width = 1,
                                    height = 1,
                                    hollow = false,
                                    color = fg ? color : (color * 0.5f),
                                });
                            }
                        }
                    }
                    break;
                }
            }
        }
        if (generatedDecals > 0) {
            Logger.Info("GameHelper", $"Added {generatedDecals} Debug Decals via GenerateDebugDecalsEvent");
        }
    }

    private static void InvokeGeneratorHooks(Session session, LevelData level, EntityData entity) {
        if (GenerateDebugDecalEvent == null) {
            return;
        }

        foreach (Delegate subscriber in GenerateDebugDecalEvent.GetInvocationList()) {
            if (subscriber is not Func<Session, LevelData, EntityData, Dictionary<string, object>> castSubscriber) {
                throw new Exception("Debug Decal Generator: wrong subscriber type");
            }

            Dictionary<string, object> newDecalData = castSubscriber(session, level, entity);

            if (newDecalData == null) {
                continue;
            }

            if (!DecalIndex.TryGetValue(level.Name, out List<DebugDecalData> list)) {
                list = new();
                DecalIndex.Add(level.Name, list);
            }

            void SafeAssign<T>(ref T dest, string key) {
                if (newDecalData.TryGetValue(key, out object read)) {
                    if (read is not T res) throw new ArgumentException($"Debug Decal Generator: wrong type for key {key}");
                    dest = res;
                }
            }

            DebugDecalData data = new();
            SafeAssign(ref data.type, "type");
            SafeAssign(ref data.position, "position");
            SafeAssign(ref data.width, "width");
            SafeAssign(ref data.height, "height");
            SafeAssign(ref data.hollow, "hollow");
            SafeAssign(ref data.thickness, "thickness");
            SafeAssign(ref data.data, "data");
            SafeAssign(ref data.scaleX, "scaleX");
            SafeAssign(ref data.scaleY, "scaleY");
            SafeAssign(ref data.color, "color");
            SafeAssign(ref data.textures, "textures");
            SafeAssign(ref data.animationSpeed, "animationSpeed");
            SafeAssign(ref data.useGui, "useGui");
            SafeAssign(ref data.rotation, "rotation");

            list.Add(data);
            generatedDecals++;
        }
    }

    private static void OnMapEditorRender(On.Celeste.Editor.MapEditor.orig_Render orig, MapEditor self) {
        if (ColorIndex == null || DecalIndex == null) {
            IndexLevel(DynamicData.For(self).Get<Session>("CurrentSession"));
        }
        orig(self);
    }

    private static void OnRenderLevel(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig, LevelTemplate self, Camera camera, List<LevelTemplate> allLevels) {
        if (ColorIndex.Count == 0 && DecalIndex.Count == 0) {
            orig(self, camera, allLevels);
            return;
        }

        if (!CullHelper.IsRectangleVisible(self.X, self.Y, self.Width, self.Height, 4f, camera)) {
            return;
        }

        if (self.Type == LevelTemplateType.Level) {
            bool vanillaDummy = false;
            if (!ColorIndex.TryGetValue(self.Name, out Color[] colors)) {
                colors = defaultColors;
                vanillaDummy = self.Dummy;
            }

            bool flash = false;
            // Red blinking if levels are intersecting
            if (Engine.Scene.BetweenInterval(0.1f)) {
                foreach (LevelTemplate allLevel in allLevels) {
                    if (allLevel != self && allLevel.Rect.Intersects(self.Rect)) {
                        flash = true;
                        break;
                    }
                }
            }
            Draw.Rect(self.X, self.Y, self.Width, self.Height, (flash ? Color.Red : colors[3]) * 0.5f);
            foreach (Rectangle back in self.backs) {
                Draw.Rect(self.X + back.X, self.Y + back.Y, back.Width, back.Height, colors[2] * 0.5f);
            }
            foreach (Rectangle solid in self.solids) {
                Draw.Rect(self.X + solid.X, self.Y + solid.Y, solid.Width, solid.Height, vanillaDummy ? Color.LightGray : colors[1]);
            }

            if (DecalIndex.TryGetValue(self.Name, out List<DebugDecalData> decals)) {
                foreach (DebugDecalData d in decals) {
                    Vector2 offset = new(self.X, self.Y);
                    switch (d.type) {
                    case TYPE_IMAGE:
                        Atlas atlas = d.useGui ? GFX.Gui : GFX.Game;
                        Image image = new(atlas[d.data]) {
                            Color = d.color,
                            Scale = new(d.scaleX / 8f, d.scaleY / 8f),
                            Rotation = d.rotation / 180f * (float) Math.PI
                        };
                        image.Position = new Vector2(d.position.X + self.X, d.position.Y + self.Y);
                        image.CenterOrigin();
                        image.Render();
                        break;

                    case TYPE_ANIMATION:
                        atlas = d.useGui ? GFX.Gui : GFX.Game;
                        image = new(
                            atlas[
                                d.textures[
                                    (int) (Engine.Scene.TimeActive / d.animationSpeed % d.textures.Count)
                                ]
                            ]
                        ) {
                            Color = d.color,
                            Scale = new(d.scaleX / 8f, d.scaleY / 8f),
                            Rotation = d.rotation / 180f * (float) Math.PI
                        };
                        image.Position = new Vector2(d.position.X + self.X, d.position.Y + self.Y);
                        image.CenterOrigin();
                        image.Render();
                        break;

                    case TYPE_RECTANGLE:
                        if (d.hollow) {
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

            foreach (Vector2 spawn in self.Spawns) {
                Draw.Rect(self.X + spawn.X, self.Y + spawn.Y - 1f, 1f, 1f, colors[0]);
            }
            foreach (Vector2 strawberry in self.Strawberries) {
                Draw.HollowRect(self.X + strawberry.X - 1f, self.Y + strawberry.Y - 2f, 3f, 3f, colors[4]);
            }
            foreach (Vector2 checkpoint in self.Checkpoints) {
                Draw.HollowRect(self.X + checkpoint.X - 1f, self.Y + checkpoint.Y - 2f, 3f, 3f, colors[5]);
            }
            foreach (Rectangle jumpthru in self.Jumpthrus) {
                Draw.Rect(self.X + jumpthru.X, self.Y + jumpthru.Y, jumpthru.Width, 1f, colors[6]);
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

    [GeneratedRegex("\\r\\n|\\n\\r|\\n|\\r")]
    private static partial Regex TileRegex();
}