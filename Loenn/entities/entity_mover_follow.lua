local mover = {}

local approachModi = {
    a_Exponential = "Exponential",
    b_Cubic = "Cubic",
    c_Quadratic = "Quadratic",
    d_Linear = "Linear",
    e_Constant = "Constant",
    f_Hyperbolic = "Hyperbolic",
    g_Inverse_Quadratic = "Inverse Quadratic",
    h_Inverse_Cubic = "Inverse Cubic",
    i_Inverse_Exponential = "Inverse Exponential"
}

mover.name = "GameHelper/EntityMoverFollow"
mover.depth = -999999999
mover.texture = "loenn/GameHelper/entity_mover_follow"
mover.justification = {0, 0}
mover.nodeLimits = {1, 1}
mover.nodeLineRenderType = "line"
mover.placements = {
    name = "follow",
    data = {
        approachMode = "Constant",
        speedFactor = 1,
        flag = "",
        onlyX = false,
        onlyY = false,
        naiveMovement = false,
        onlyType = "",
        targetOnlyType = "",
        approachSound = "",
        playSoundAtDistance = 100.0,
        stopSoundOnLeave = false,
        holdPositionOnWait = false,
        debug = false,
        awaitPlayerMovement = true,
        minDistance = 0.0,
        maxDistance = 0.0,
        offsetX = 0.0,
        offsetY = 0.0
    }
}
mover.fieldOrder = {
    "x", "y",
    "approachMode", "speedFactor",  "onlyType", "targetOnlyType", "minDistance", "maxDistance",
    "offsetX", "offsetY", "approachSound", "playSoundAtDistance", "flag",
    "onlyX", "onlyY", "naiveMovement", "holdPositionOnWait","awaitPlayerMovement", "stopSoundOnLeave", "debug"
}
mover.fieldInformation = {
    approachMode = {
        options = approachModi,
        editable = false
    }
}

return mover