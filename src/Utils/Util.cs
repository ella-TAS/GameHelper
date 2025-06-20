using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.GameHelper.Utils;

public static class Util {
    private static Dictionary<string, Ease.Easer> Easers;

    public static void DrawCircle(Vector2 center, float radius, Color color) {
        float radius2 = (radius * radius) - 0.25f;
        int y = (int) Math.Round(radius);
        drawCirclePx(0, y);
        for(int x = 1; x < y; x++) {
            //(y-0.5)² = y²-y+0.25
            if((x * x) + (y * y) - y > radius2) {
                y--;
            }
            drawCirclePx(x, y);
        }

        void drawCirclePx(int x, int y) {
            Draw.Point(center + new Vector2(x, y), color);
            Draw.Point(center + new Vector2(-x, y), color);
            Draw.Point(center + new Vector2(x, -y), color);
            Draw.Point(center + new Vector2(-x, -y), color);
            Draw.Point(center + new Vector2(y, x), color);
            Draw.Point(center + new Vector2(-y, x), color);
            Draw.Point(center + new Vector2(y, -x), color);
            Draw.Point(center + new Vector2(-y, -x), color);
        }
    }

    public static Color ColorInterpolate(Color start, Color end, float value) {
        return new(
            calculate(start.R, end.R, value),
            calculate(start.G, end.G, value),
            calculate(start.B, end.B, value)
        );

        static int calculate(int start, int end, float value) {
            return Calc.Clamp((int) (start * (1 - value) + end * value), 0, 255);
        }
    }

    public static float EaseMode(float s, string mode) {
        if(Easers.TryGetValue(mode, out Ease.Easer value)) {
            return value(s);
        }
        Logger.Warn("GameHelper", "Ease Mode " + mode + " not found");
        return s;
    }

    public static void Load() {
        Easers = typeof(Ease).GetFields(BindingFlags.Static | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(Ease.Easer))
            .ToDictionary(f => f.Name, f => (Ease.Easer) f.GetValue(null), StringComparer.OrdinalIgnoreCase);
    }

    public static bool GetFlag(string flag, Scene s, bool emptyReaction = false, bool invert = false) {
        if(flag?.Length == 0) return emptyReaction;
        return (s as Level).Session.GetFlag(flag) ^ invert;
    }

    public static string GetCelesteSaveName(int slot) {
        string s = SaveData.GetFilename(slot);
        return UserIO.Exists(s) ? UserIO.Load<SaveData>(s).Name : "";
    }
}