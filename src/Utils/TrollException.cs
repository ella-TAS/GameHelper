using System;

namespace Celeste.Mod.GameHelper.Utils;

public class TrollException : Exception {
    public TrollException(string message) : base(message) { }
}