using Celeste.Mod.GameHelper.Entities.Controllers;
using Celeste.Mod.GameHelper.Features.DebugMap;
using MonoMod.ModInterop;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.GameHelper;

[ModExportName("GameHelper")]
public static class GameHelperExports {
    /// <summary>
    /// Adds a custom button binding to the GameHelper Flashlight Controller
    /// </summary>
    /// <param name="levelSID">The level SID of the level where the button binding should apply</param>
    /// <param name="binding">The custom ButtonBinding that should be applied</param>
    /// public static Action<string, ButtonBinding> AddFlashlightBinding;
    public static void AddFlashlightBinding(string levelSID, ButtonBinding binding) {
        FlashlightController.addBinding(levelSID, binding);
    }

    /// <summary>
    /// Resets all custom button bindings for the GameHelper Flashlight Controller
    /// </summary>
    /// public static Action ResetFlashlightBindings;
    public static void ResetFlashlightBindings() {
        FlashlightController.resetBindings();
    }

    /// <summary>
    /// Adds a custom button binding to the GameHelper Player Shadow Controller
    /// </summary>
    /// <param name="levelSID">The level SID of the level where the button binding should apply</param>
    /// <param name="binding">The custom ButtonBinding that should be applied</param>
    /// public static Action<string, ButtonBinding> AddPlayerShadowBinding;
    public static void AddPlayerShadowBinding(string levelSID, ButtonBinding binding) {
        PlayerShadowController.addBinding(levelSID, binding);
    }

    /// <summary>
    /// Resets all custom button bindings for the GameHelper Player Shadow Controller
    /// </summary>
    /// public static Action ResetPlayerShadowBindings;
    public static void ResetPlayerShadowBindings() {
        PlayerShadowController.resetBindings();
    }

    /// <summary>
    /// Hook the GenerateDebugDecalEvent that is called when an entity is indexed for addition to the debug map.
    /// Use this to generate map-wide debug decals for entities.
    /// Please check for session.Area.SID to restrict behavior to your map.
    /// Return null unless you want to add a decal.
    /// Please call the Remove function in Unload().
    /// Hook footprint:
    /// public static Dictionary<string, object> GenerateDebugDecal(Session session, LevelData level, EntityData entity)
    /// </summary>
    /// <param name="generator">The function gets the session, level data and entity data and should return a string-object Dictionary with the following possible values:
    /// type : string
    /// position : Vector2
    /// width : int
    /// height : int
    /// hollow : bool
    /// thickness : float
    /// data : string
    /// scaleX : float
    /// scaleY : float
    /// color : Color
    /// textures : List<string>
    /// animationSpeed : float
    /// useGui : bool
    /// rotation : float
    /// </param>
    /// public static Action<Func<Session, LevelData, EntityData, Dictionary<string, object>>> AddGenerateDebugDecalEvent;
    public static void AddGenerateDebugDecalEvent(Func<Session, LevelData, EntityData, Dictionary<string, object>> generator) {
        ColorfulDebug.GenerateDebugDecalEvent += generator;
    }

    /// <summary>
    /// Unhooks the GenerateDebugDecalEvent. Please call this in Unload().
    /// </summary>
    /// <param name="generator">The hook to be removed</param>
    /// public static Action<Func<Session, LevelData, EntityData, Dictionary<string, object>>> RemoveGenerateDebugDecalEvent;
    public static void RemoveGenerateDebugDecalEvent(Func<Session, LevelData, EntityData, Dictionary<string, object>> generator) {
        ColorfulDebug.GenerateDebugDecalEvent -= generator;
    }
}