local utils = require("utils")

local trampoline = {}

trampoline.name = "GameHelper/Trampoline"
trampoline.depth = 8998
trampoline.texture = "objects/GameHelper/trampoline/idle"
trampoline.placements = {
    name = "trampoline",
    data = {
        speedBoostX = 60.0,
        speedBoostY = 60.0,
        facingUpLeft = true,
        refillDash = true,
        oneUse = false
    }
}

function trampoline.rectangle(room, entity, viewport)
    return utils.rectangle(entity.x, entity.y, 16, 16)
end

function trampoline.justification(room, entity)
    return 0.32, (entity.facingUpLeft and 0.32 or 0.95)
end

function trampoline.rotation(room, entity)
    return entity.facingUpLeft and 0 or math.pi / 2
end

return trampoline