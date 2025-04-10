local utils = require("utils")

local colorOptions = {
    Red = "red",
    Blue = "blue",
    Green = "green",
    Yellow = "yellow"
}

local balloon = {}

balloon.name = "GameHelper/Balloon"
balloon.depth = 8998
balloon.justification = {0.0, 0.18}
balloon.fieldInformation = {
    color = {
        options = colorOptions,
        editable = false
    }
}
balloon.placements = {
    name = "balloon",
    data = {
        color = "red",
        superBounce = false,
        oneUse = false
    }
}

function balloon.rectangle(room, entity, viewport)
    return utils.rectangle(entity.x, entity.y - 4, 15, 17)
end

function balloon.texture(room, entity)
    return "loenn/GameHelper/balloon_" .. entity.color
end

return balloon