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
    {
        name = "simple",
        data = {
            moveTime = 5.0,
            flag = "",
            setFlagOnEnd = "",
            returnType = "Stop",
            additiveMovement = false,
            easeMode = "Linear",
            onlyType = "",
            debug = false,
            naiveMovement = false,
            nodeSound = "",
            holdPositionOnWait = false
        }
    }, {
        name = "nodeWait",
        data = {
            moveTime = 5.0,
            nodeWaitTime = 0.0,
            flag = "",
            setFlagOnEnd = "",
            returnType = "Stop",
            additiveMovement = false,
            easeMode = "Linear",
            onlyType = "",
            debug = false,
            naiveMovement = false,
            holdPositionOnWait = false,
            moveSound = "",
            nodeSound = "",
            stopMoveSoundOnStop = false
        }
    }
}
mover.fieldOrder = {
    "x", "y",
    "moveTime", "nodeWaitTime", "onlyType", "returnType", "easeMode", "flag", "setFlagOnEnd", "moveSound", "nodeSound",
    "additiveMovement", "naiveMovement", "stopMoveSoundOnStop", "holdPositionOnWait", "debug"
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