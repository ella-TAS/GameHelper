using Microsoft.Xna.Framework;
using Celeste.Mod.Entities;

namespace Celeste.Mod.GameHelper.Triggers;

[CustomEntity("GameHelper/CrashGameTrigger")]
public class CrashGameTrigger : Trigger {
    private string message;

    public CrashGameTrigger(EntityData data, Vector2 levelOffset) : base(data, levelOffset) {
        message = data.Attr("message");
    }

    public override void OnEnter(Player p) {
        throw (new TrollException(message));
    }
}

public class TrollException : System.Exception {
    public TrollException(string message) : base(message) { }
}