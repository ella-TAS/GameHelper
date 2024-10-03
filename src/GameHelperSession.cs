using YamlDotNet.Serialization;
using System.Collections.Generic;
using Celeste.Mod.GameHelper.Entities;
using Celeste.Mod.GameHelper.Utils.Components;

namespace Celeste.Mod.GameHelper;

public class GameHelperSession : EverestModuleSession {
    public bool FloatyJumps;
    public bool HeartTriggerActivated;
    [YamlIgnore]
    public bool PlayerHasShield;
    [YamlIgnore]
    public List<FlagCollectBerry> StoredBerries;
    [YamlIgnore]
    public Dictionary<string, PSwitchTimer> PSwitchTimers;
}