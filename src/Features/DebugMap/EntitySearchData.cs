using System.Collections.Generic;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public static class EntitySearchData {
    public readonly static Dictionary<string, string> Aliases = new() {
        { "badelineBoss", "finalBoss" },
        { "badelineBossFallingBlock", "finalBossFallingBlock" },
        { "badelineBossMovingBlock", "finalBossMovingBlock" },
        { "badelineChaser", "darkChaser" },
        { "badelineChaserBarrier", "darkChaserEnd" },
        { "binocular", "towerviewer" },
        { "bubble", "booster" },
        { "bumper", "bigSpinner" },
        { "clutterBlockBooks", "greenBlocks" },
        { "clutterBlockBoxes", "yellowBlocks" },
        { "clutterBlockTowels", "redBlocks" },
        { "clutterSwitch", "colorSwitch" },
        { "conveyor", "wallBooster" },
        { "coreBlock", "bounceBlock" },
        { "coreSwitch", "coreModeToggle" },
        { "crystalHeart", "blackGem" },
        { "crystalHeart1A", "birdForsakenCityGem" },
        { "dashCrystal", "refill" },
        { "deathPlane", "killbox" },
        { "feather", "infiniteStar" },
        { "heartGate", "heartGemDoor" },
        { "iceBall", "fireBall" },
        { "iceWall", "wallBooster" },
        { "internetCafe", "wavedashmachine" },
        { "jellyfish", "glider" },
        { "kevin", "crushBlock" },
        { "keyDoor", "lockBlock" },
        { "lavaBlock", "fireBarrier" },
        { "lookout", "towerviewer" },
        { "oshiroBoss", "friendlyGhost" },
        { "powerBox", "lightningBlock" },
        { "pufferfish", "eyebomb" },
        { "spaceJam", "dreamBlock" },
        { "strawberryBlockfield", "blockField" },
        { "summitFlag", "summitcheckpoint" },
        { "switchBlock", "swapBlock" },
        { "swapBlock", "switchBlock" },
        { "watchtower", "towerviewer" },
        { "wingedGoldenBerry", "memorialTextController" },

        // triggers
        { "binocularBlocker", "lookoutBlocker" },
        { "snowballTrigger", "windAttackTrigger" },
        { "watchtowerBlocker", "lookoutBlocker " }
    };

    public readonly static Dictionary<string, string[]> Groups = new() {
        { "AllEntities", [] },
        { "VanillaEntities", [] },
        { "ModdedEntities", [] },
        { "Gameplay[75]", ["refill", "infiniteStar", "spring", "wallSpringLeft", "wallSpringRight", "fallingBlock", "zipMover", "crumbleBlock", "dreamBlock", "touchSwitch", "switchGate", "negaBlock", "movingPlatform", "rotatingPlatforms", "cloud", "booster", "moveBlock", "switchBlock", "swapBlock", "dashSwitchH", "dashSwitchV", "templeGate", "theoCrystal", "glider", "badelineBoost", "cassetteBlock",
            "wallBooster", "bounceBlock", "coreModeToggle", "iceBlock", "fireBarrier", "eyebomb", "flingBird", "lightningBlock", "spikesUp", "spikesDown", "spikesLeft", "spikesRight", "triggerSpikesUp", "triggerSpikesDown", "triggerSpikesRight", "triggerSpikesLeft", "darkChaser", "rotateSpinner", "trackSpinner", "spinner", "friendlyGhost", "seeker", "slider", "crushBlock", "bigSpinner", "fireBall",
            "risingLava", "sandwichLava", "killbox", "fakeHeart", "lightning", "finalBoss", "finalBossFallingBlock", "finalBossMovingBlock", "dashBlock", "invisibleBarrier", "playerSeeker", "introCrusher", "bridge", "colorSwitch", "clutterDoor", "yellowBlocks", "redBlocks", "greenBlocks", "oshirodoor", "whiteblock", "water", "SummitBackgroundManager", "heartGemDoor", "blackGem"] },
        { "Solids[39]", ["fallingBlock", "zipMover", "crumbleBlock", "dreamBlock", "switchGate", "negaBlock", "lockBlock", "moveBlock", "switchBlock", "swapBlock", "templeGate", "templeCrackedBlock", "cassetteBlock", "bounceBlock", "iceBlock", "fireBarrier", "lightningBlock", "crushBlock", "starJumpBlock", "floatySpaceBlock", "glassBlock", "goldenBlock", "finalBossFallingBlock",
            "finalBossMovingBlock", "dashBlock", "invisibleBarrier", "exitBlock", "conditionBlock", "crumbleWallOnRumble", "ridgeGate", "introCrusher", "colorSwitch", "clutterDoor", "yellowBlocks", "redBlocks", "greenBlocks", "oshirodoor", "heartGemDoor", "dashSwitchH", "dashSwitchV"] },
        { "Hazards[23]", ["iceBlock", "fireBarrier", "spikesUp", "spikesDown", "spikesLeft", "spikesRight", "triggerSpikesUp", "triggerSpikesDown", "triggerSpikesRight", "triggerSpikesLeft", "darkChaser", "rotateSpinner", "trackSpinner", "spinner", "friendlyGhost", "seeker", "slider", "fireBall", "risingLava", "sandwichLava", "killbox", "lightning", "finalBoss"] },
        { "Spikes[8]", ["spikesUp", "spikesDown", "spikesLeft", "spikesRight", "triggerSpikesUp", "triggerSpikesDown", "triggerSpikesLeft", "triggerSpikesRight"] },
        { "Springs[3]", ["spring", "wallSpringLeft", "wallSpringRight"] },

        { "AllTriggers", [] },
        { "MusicTriggers[4]", ["musicFadeTrigger", "musicTrigger", "altMusicTrigger", "ambienceParamTrigger"] },
        { "CameraTriggers[3]", ["cameraOffsetTrigger", "cameraTargetTrigger", "cameraAdvanceTargetTrigger"] }
    };

    public readonly static Dictionary<string, int[]> SpecificOffset = new() {
        { "spikesDown", [0, 1] },
        { "spikesLeft", [-1, 0] },
        { "jumpThru", [0, 1] }
    };
}