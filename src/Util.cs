using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.GameHelper;

public class Util {
    private static Dictionary<string, Ease.Easer> Easers;

    public static float EaseMode(float s, string mode) {
        if(Easers.ContainsKey(mode)) {
            return Easers[mode](s);
        }
        Logger.Warn("GameHelper", "Ease Mode " + mode + " not found");
        return s;
    }

    public static void Load() {
        Easers = typeof(Ease).GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(Ease.Easer))
            .ToDictionary(f => f.Name, f => (Ease.Easer) f.GetValue(null), StringComparer.OrdinalIgnoreCase);
    }
}