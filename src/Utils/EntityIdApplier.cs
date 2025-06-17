using Celeste.Mod.GameHelper.Utils.Components;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Utils;

internal static class EntityIdApplier {
    public static List<Entity> addTracker = [];
    public static int? addId = null;

    private static void OnLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level level, Player.IntroTypes intro, bool fromLoader) {
        addId = null;
        orig(level, intro, fromLoader);
        if(addId != null) {
            addTracker.ForEach(e => {
                if(e is Decal || e is Player || (e is Key && (e as Key).follower.HasLeader)) return;
                e.Add(new EntityIdComponent(addId.GetValueOrDefault()));
            });
        }
        addId = null;
    }

    private static bool OnLoadCustomEntity(On.Celeste.Level.orig_LoadCustomEntity orig, EntityData data, Level level) {
        // previous entity
        if(addId != null) {
            addTracker.ForEach(e => e.Add(new EntityIdComponent(addId.GetValueOrDefault())));
            addId = null;
        }

        // current entity
        addTracker = [];
        bool loaded = orig(data, level);
        if(loaded) {
            addTracker.ForEach(e => e.Add(new EntityIdComponent(data.ID)));
        } else {
            addTracker = [];
            addId = data.ID;
        }
        return loaded;
    }

    private static void OnAddEntity(On.Monocle.Scene.orig_Add_Entity orig, Scene scene, Entity entity) {
        addTracker.Add(entity);
        orig(scene, entity);
    }

    public static void Hook() {
        On.Celeste.Level.LoadLevel += OnLoadLevel;
        On.Celeste.Level.LoadCustomEntity += OnLoadCustomEntity;
        On.Monocle.Scene.Add_Entity += OnAddEntity;
    }

    public static void Unhook() {
        On.Celeste.Level.LoadLevel -= OnLoadLevel;
        On.Celeste.Level.LoadCustomEntity -= OnLoadCustomEntity;
        On.Monocle.Scene.Add_Entity -= OnAddEntity;
    }
}