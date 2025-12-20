using Celeste.Mod.GameHelper.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Features;

public static class Commands {
    [Command("clear_area", "Collect all berries and mark the area as cleared. For testing purposes.")]
    public static void CmdCompleteArea() {
        Level level = (Level) Engine.Scene;
        Util.CollectBerries(level.Tracker.GetEntity<Player>());
        level.RegisterAreaComplete();
    }

    [Command("auto_save", "Auto save.")]
    public static void CmdAutoSave() {
        Level level = (Level) Engine.Scene;
        level.AutoSave();
    }

    [Command("entities", "Output a list of entities for Debug purposes")]
    public static void CmdEntityList(bool full = false) {
        Level level = (Level) Engine.Scene;

        Dictionary<string, int> dict = [];
        foreach (Entity e in level.Entities) {
            string key = full ? e.GetType().FullName : e.GetType().Name;
            if (!dict.ContainsKey(key)) {
                dict[key] = 0;
            }
            dict[key]++;
        }

        if (dict.Count == 0) {
            Engine.Commands.Log("No entities found.", Color.Red);
            return;
        }

        IOrderedEnumerable<KeyValuePair<string, int>> sortedDict = from entry in dict orderby entry.Value descending, entry.Key ascending select entry;

        string result = "";
        foreach (KeyValuePair<string, int> pair in sortedDict) {
            if (pair.Value > 1) {
                result += $"{pair.Value} * ";
            }
            result += $"{pair.Key}, ";
        }

        Engine.Commands.Log(result[..^2]);
    }
}