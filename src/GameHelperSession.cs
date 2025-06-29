using Celeste.Mod.GameHelper.Entities;
using Celeste.Mod.GameHelper.Utils.Components;
using System.Collections.Generic;
using YamlDotNet.Serialization;

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

    // [X, Y, Width, Height, ID]
    [YamlIgnore]
    public SortedDictionary<string, List<int[]>> EntityIndex;
    [YamlIgnore]
    public SortedDictionary<string, List<int[]>> TriggerIndex;
    [YamlIgnore]
    public Dictionary<string, List<int[]>> GroupIndex;
    [YamlIgnore]
    public List<string> RecentSearch;
}