using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.GameHelper.Utils;

public class Coroutines {
    public static Coroutine Timeout(Action action, float? timeout = null) {
        return new Coroutine(timeoutCoroutine(action, timeout));
    }

    private static IEnumerator timeoutCoroutine(Action action, float? timeout) {
        yield return timeout;
        action();
    }
}