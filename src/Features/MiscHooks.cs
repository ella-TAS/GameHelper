using Celeste.Mod.GameHelper.Utils;
using System;

namespace Celeste.Mod.GameHelper.Features;

public static class MiscHooks {
    private static bool? isParrot = null;

    private static string DialogClean(On.Celeste.Dialog.orig_Clean orig, string name, Language language) {
        isParrot ??= Util.GetCelesteSaveName(0).Equals("Chat") &&
            Util.GetCelesteSaveName(1).Equals("Birbuh") &&
            Util.GetCelesteSaveName(2).Equals("hi chat");

        // 21. June, on Parrot's install
        if(isParrot == true && DateTime.Now.Month == 6 && DateTime.Now.Day is >= 21 and <= 23) {
            switch(name.ToUpper()) {
                case "MENU_BEGIN":
                    return "PARROT";
                case "FILE_BEGIN":
                case "FILE_CONTINUE":
                case "OVERWORLD_NORMAL":
                    return "DASH";
            }
        }
        return orig(name, language);
    }

    public static void Hook() {
        On.Celeste.Dialog.Clean += DialogClean;
    }

    public static void Unhook() {
        On.Celeste.Dialog.Clean -= DialogClean;
    }
}