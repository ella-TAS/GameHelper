using YamlDotNet.Serialization;
using System.Collections.Generic;
using Celeste.Mod.GameHelper.Entities;

namespace Celeste.Mod.GameHelper;

public class GameHelperSession : EverestModuleSession {
    public bool FloatyJumps;
    public bool HeartTriggerActivated;
    [YamlIgnore]
    public List<FlagCollectBerry> StoredBerries;
}