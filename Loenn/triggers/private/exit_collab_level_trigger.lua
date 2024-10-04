local elt = {}

elt.name = "GameHelper/ExitCollabLevelTrigger"
elt.depth = 8998
elt.placements = {
    {
        name = "withTrigger",
        data = {
            addHeartTrigger = true,
            delay = 1.0,
            timeRate = 0.5,
            flag = ""
        }
    },
    {
        name = "withoutTrigger",
        data = {
            addHeartTrigger = false,
            delay = 0.0,
            timeRate = 0.5,
            flag = ""
        }
    }
}

return elt