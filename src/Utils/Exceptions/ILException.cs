using System;

namespace Celeste.Mod.GameHelper.Utils.Exceptions;

public class ILException(string location) : Exception("ILHook on " + location + " failed") { }