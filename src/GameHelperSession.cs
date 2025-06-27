using Celeste.Mod.GameHelper.Entities;
using Celeste.Mod.GameHelper.Utils.Components;
using Microsoft.Xna.Framework;
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
    [YamlIgnore]
    // [X, Y, Width, Height, ID]
    public SortedDictionary<string, List<int[]>> EntitySearchIndex;
}