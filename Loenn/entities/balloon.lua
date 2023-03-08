local utils = require("utils")

local balloon = {}

balloon.name = "GameHelper/Balloon"
balloon.depth = 8998
balloon.texture = "loenn/GameHelper/balloon"
balloon.justification = {0.0, 0.18}
balloon.placements = {
    name = "balloon",
    data = {}
}

function balloon.rectangle(room, entity, viewport)
    return utils.rectangle(entity.x, entity.y - 4, 15, 17)
end

return balloon