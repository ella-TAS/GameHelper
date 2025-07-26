local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local ddc = {}

ddc.name = "GameHelper/DebugDecalController"
ddc.texture = "loenn/GameHelper/debug_decal_controller"
ddc.depth = -999999999
ddc.justification = {0.5, 0.5}
ddc.nodeLineRenderType = "line"
ddc.placements = {
    {
        name = "image",
        data = {
            type = "Image",
            sprite = "",
            scaleX = 1.0,
            scaleY = 1.0,
            color = "FFFFFF",
            useGui = false,
            rotation = 0.0
        }
    },
    {
        name = "animation",
        data = {
            type = "Animation",
            spriteName = "",
            scaleX = 1.0,
            scaleY = 1.0,
            color = "FFFFFF",
            animationSpeed = 0.1,
            useGui = false,
            rotation = 0.0
        }
    },
    {
        name = "rectangle",
        data = {
            type = "Rectangle",
            hollow = false,
            color = "FFFFFF"
        }
    },
    {
        name = "line",
        data = {
            type = "Line",
            thickness = 1.0,
            color = "FFFFFF"
        }
    },
    {
        name = "text",
        data = {
            type = "Text",
            dialog = "",
            color = "FFFFFF",
            scale = 1.0
        }
    }
}

ddc.fieldInformation = {
    color = { fieldType = "color" },
}

ddc.ignoredFields = {
    "_id", "_name", "originX", "originY", "type"
}

function ddc.canResize(room, entity)
    if entity.type == "Rectangle" then
        return {true, true}
    else
        return {false, false}
    end
end

function ddc.nodeLimits(room, entity)
    if entity.type == "Line" then
        return {1, 1}
    else
        return {0, 0}
    end
end

function ddc.color(room, entity)
    local color = {1, 1, 1}
    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)
        if success then
            color = {r, g, b}
        end
    end
    return color
end

return ddc