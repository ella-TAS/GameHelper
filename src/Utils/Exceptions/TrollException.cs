using System;

namespace Celeste.Mod.GameHelper.Utils.Exceptions;

public class TrollException(string message) : Exception(message) { }