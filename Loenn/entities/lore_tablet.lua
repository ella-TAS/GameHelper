local trigger = {}

trigger.name = "GameHelper/LoreTablet"
trigger.nodeLimits = {0, 1}

trigger.placements = {
    name = "trigger",
    data = {
        dialog = "",
		flag = "",
		flagValue = true,
		onlyOnce = false,
        activateSfx = 
        loopSfx = "event:/none"
        audioSfx = "event:/none"
    }
}

return trigger