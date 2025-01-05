local mover = {}
local returnTypes = {
    Remove = "Remove",
    Teleport = "Teleport",
    Move_Start = "Move_Start",
    Move_Path = "Move_Path",
    Stop = "Stop"
}

mover.name = "GameHelper/DecalMover"
mover.depth = -999999999
mover.texture = "loenn/GameHelper/entity_mover_decal"
mover.justification = {0, 0}
mover.nodeLimits = {1, -1}
mover.nodeLineRenderType = "line"
mover.placements = {
    name = "decal",
    data = {
        speed = 60.0,
        flag = "",
        returnType = "Teleport",
        flipX = false,
        flipY = false
    }
}
mover.fieldInformation = {
    returnType = {
        options = returnTypes,
        editable = false
    }
}

return mover