using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public static class EntitySearchData {
    public readonly static Dictionary<string, string> Aliases = new() {
        {"badelineBoss", "finalBoss"},
        {"badelineBossFallingBlock", "finalBossFallingBlock"},
        {"badelineBossMovingBlock", "finalBossMovingBlock"},
        {"badelineChaser", "darkChaser"},
        {"badelineChaserBarrier", "darkChaserEnd"},
        {"binocular", "towerviewer"},
        {"bubble", "booster"},
        {"bumper", "bigSpinner"},
        {"clutterBlockBooks", "greenBlocks"},
        {"clutterBlockBoxes", "yellowBlocks"},
        {"clutterBlockTowels", "redBlocks"},
        {"clutterSwitch", "colorSwitch"},
        {"conveyor", "wallBooster"},
        {"coreBlock", "bounceBlock"},
        {"coreSwitch", "coreModeToggle"},
        {"crystalHeart", "blackGem"},
        {"crystalHeart1A", "birdForsakenCityGem"},
        {"dashCrystal", "refill"},
        {"deathPlane", "killbox"},
        {"feather", "infiniteStar"},
        {"heartGate", "heartGemDoor"},
        {"iceBall", "fireBall"},
        {"iceWall", "wallBooster"},
        {"internetCafe", "wavedashmachine"},
        {"jellyfish", "glider"},
        {"kevin", "crushBlock"},
        {"keyDoor", "lockBlock"},
        {"lavaBlock", "fireBarrier"},
        {"lookout", "towerviewer"},
        {"oshiroBoss", "friendlyGhost"},
        {"powerBox", "lightningBlock"},
        {"pufferfish", "eyebomb"},
        {"spaceJam", "dreamBlock"},
        {"strawberryBlockfield", "blockField"},
        {"summitFlag", "summitcheckpoint"},
        {"watchtower", "towerviewer"},
        {"wingedGoldenBerry", "memorialTextController"},

        // triggers
        {"binocularBlocker", "lookoutBlocker"},
        {"snowballTrigger", "windAttackTrigger"},
        {"watchtowerBlocker", "lookoutBlocker"}
    };
}