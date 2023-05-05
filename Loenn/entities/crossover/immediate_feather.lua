local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local feather = {}

feather.name = "GameHelper/ImmediateFeather"
feather.depth = 8998
feather.placements = {
    {
        name = "normal",
        data = {
            startBoost = false,
            shielded = false,
            singleUse = true,
            flightDuration = 2.0
        }
    },
    {
        name = "boost",
        data = {
            startBoost = true,
            shielded = false,
            singleUse = false,
            flightDuration = 1.0
        }
    }
}

function feather.draw(room, entity, viewport)
    if entity.shielded then
        local x, y = entity.x or 0, entity.y or 0
        love.graphics.circle("line", x, y, 12)
    end
    drawableSprite.fromTexture("objects/flyFeather/idle00", entity):draw()
end

function feather.selection(room, entity)
    if entity.shielded then
        return utils.rectangle(entity.x - 12, entity.y - 12, 24, 24)
    else
        return drawableSprite.fromTexture("objects/flyFeather/idle00", entity):getRectangle()
    end
end

return feather