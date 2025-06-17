using System;

namespace Celeste.Mod.GameHelper.Utils;

public static partial class MiscHooks {
    private static string DialogClean(On.Celeste.Dialog.orig_Clean orig, string name, Language language) {
        // 21. June
        if(DateTime.Now.Month == 6 && DateTime.Now.Day == 21) {
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