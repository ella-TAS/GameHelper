local easingOptions = require("mods").requireFromPlugin("easing_options")

local mover = {}
local returnTypes = {
    Remove = 0,
    Teleport = 1,
    Move_Start = 2,
    Move_Path = 3,
    Stop = 4
}

mover.name = "GameHelper/DecalMover"
mover.depth = -9999999
mover.texture = "loenn/GameHelper/entity_mover_decal"
mover.justification = {0.0, 0.0}
mover.nodeLimits = {1, -1}
mover.nodeLineRenderType = "line"
mover.placements = {
    name = "decal",
    data = {
        speed = 60.0,
        moveTime = 5.0,
        flag = "",
        returnType = 1,
        flipX = false,
        flipY = false,
        easeMode = "Linear",
        debug = false
    }
}
mover.fieldInformation = {
    returnType = {
        options = returnTypes
    }
}

return mover