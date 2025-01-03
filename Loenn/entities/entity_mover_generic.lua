local easingOptions = require("mods").requireFromPlugin("easing_options")
local mover = {}
local returnTypes = {
    Remove = "Remove",
    Teleport = "Teleport",
    Move_Start = "Move_Start",
    Move_Path = "Move_Path",
    Stop = "Stop"
}

mover.name = "GameHelper/EntityMover"
mover.depth = -9999999
mover.texture = "loenn/GameHelper/entity_mover_generic"
mover.justification = {0, 0}
mover.nodeLimits = {1, -1}
mover.nodeLineRenderType = "line"
mover.placements = {
    name = "generic",
    data = {
        moveTime = 5.0,
        flag = "",
        setFlagOnEnd = "",
        returnType = "Stop",
        additiveMovement = false,
        easeMode = "Linear",
        onlyType = "",
        debug = false,
        naiveMovement = false
    }
}
mover.fieldInformation = {
    returnType = {
        options = returnTypes,
        editable = false
    },
    easeMode = {
        options = easingOptions,
        editable = false
    }
}

return mover