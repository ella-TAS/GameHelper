local utils = require("utils")
local trap = {}

trap.name = "GameHelper/FireTrap"
trap.depth = -9999999
trap.texture = "objects/GameHelper/fire_trap"
trap.justification = {0.25, 0.5}
trap.placements = {
    name = "trap",
    data = {
        delay = 0.067,
        sprite = "objects/GameHelper/fire_trap"
    }
}

function trap.rectangle(room, entity, viewport)
    return utils.rectangle(entity.x, entity.y, 8, 8)
end

return trap