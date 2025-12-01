using MonoMod.ModInterop;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper;

[ModImportName("CollabUtils2.ReturnToLobbyHelper")]
public static class CollabUtils2Imports {
    /// <summary>
    /// Displays the endscreen of the individual collab map if the player enabled it, pauses for up to 1 second otherwise.
    /// This is the same behavior as mini hearts.
    /// </summary>
    /// <returns>A coroutine that should be run through in order to show the endscreen and wait for user input</returns>
    public static Func<IEnumerator> DisplayCollabMapEndScreenIfEnabled;

    /// <summary>
    /// Triggers the transition that sends the player back to the lobby from an individual collab map,
    /// like mini hearts do.
    /// </summary>
    public static Action TriggerReturnToLobby;
}