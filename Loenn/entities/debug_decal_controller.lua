local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local drawableText = require("structs.drawable_text")
local ddc = {}

ddc.name = "GameHelper/DebugDecalController"
ddc.justification = { 0.5, 0.5 }
ddc.nodeLineRenderType = "line"
ddc.depth = -99999
ddc.placements = {
    {
        name = "image",
        data = {
            type = "Image",
            sprite = "",
            scaleX = 8.0,
            scaleY = 8.0,
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
            scaleX = 8.0,
            scaleY = 8.0,
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
            color = "FFFFFF",
            width = 16,
            height = 16
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
            scale = 8.0
        }
    }
}

ddc.fieldInformation = {
    color = { fieldType = "color" },
}

ddc.ignoredFields = {
    "_id", "_name", "originX", "originY", "type"
}

ddc.fieldOrder = {
    "x", "y", "color", "rotation", "scaleX", "scaleY", "sprite", "spriteName", "animationSpeed"
}

function ddc.canResize(room, entity)
    if entity.type == "Rectangle" then
        return { true, true }
    else
        return { false, false }
    end
end

function ddc.nodeLimits(room, entity)
    if entity.type == "Line" then
        return { 1, 1 }
    else
        return { 0, 0 }
    end
end

function ddc.color(room, entity)
    local color = { 1, 1, 1 }
    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)
        if success then
            color = { r, g, b }
        end
    end
    return color
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function ddc.draw(room, entity, viewport)
    local r, g, b = hexToRGB(entity.color)
    love.graphics.setColor(r, g, b, 0.5)

    if entity.type == "Image" or entity.type == "Animation" then
        local debugImage
        if entity.type == "Image" then
            debugImage = drawableSprite.fromTexture(entity.sprite, entity)
        else
            -- try various combinations for animated sprites
            debugImage = drawableSprite.fromTexture(entity.spriteName, entity)
            if debugImage == nil then
                debugImage = drawableSprite.fromTexture(entity.spriteName .. "0", entity)
            end
            if debugImage == nil then
                debugImage = drawableSprite.fromTexture(entity.spriteName .. "00", entity)
            end
            if debugImage == nil then
                debugImage = drawableSprite.fromTexture(entity.spriteName .. "000", entity)
            end
        end
        if debugImage ~= nil then
            debugImage.rotation = (entity.rotation or 0) * math.pi / 180.0
            debugImage:draw()
        else
            debugImage = drawableSprite.fromTexture("loenn/GameHelper/debug_decal_controller", entity)
            debugImage:setScale(1, 1)
            debugImage:draw()
        end
    elseif entity.type == "Rectangle" then
        if not entity.hollow then
            love.graphics.setColor(r, g, b, 0.2)
            love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
        end
        love.graphics.setColor(r, g, b, 1.0)
        love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)
    elseif entity.type == "Text" then
        if entity.dialog ~= "" then
            local offsetX = love.graphics.getFont():getWidth(entity.dialog) * entity.scale * 8 / 2
            local offsetY = love.graphics.getFont():getHeight(entity.dialog) * entity.scale * 8 / 2
            love.graphics.print(entity.dialog, entity.x - offsetX, entity.y - offsetY, 0.0, entity.scale * 8,
                entity.scale * 8)
        else
            local default = drawableSprite.fromTexture("loenn/GameHelper/debug_decal_controller", entity)
            default:setScale(1, 1)
            default:draw()
        end
    elseif entity.type == "Line" and (entity.thickness == 0.0 or (entity.x == entity.nodes[1].x and entity.y == entity.nodes[1].y)) then
        -- rest of Line is handled below in node
        local debugImage = drawableSprite.fromTexture("loenn/GameHelper/debug_decal_controller", entity)
        debugImage:draw()
    end
    love.graphics.setColor(1, 1, 1, 1)
end

function ddc.nodeDraw(room, entity, node, nodeIndex, viewport)
    local r, g, b = hexToRGB(entity.color)
    love.graphics.setColor(r, g, b, 0.5)
    local width = love.graphics.getLineWidth()
    love.graphics.setLineWidth(entity.thickness * 8.0)
    love.graphics.line(entity.x, entity.y, node.x, node.y)
    love.graphics.setLineWidth(width)
    love.graphics.setColor(1, 1, 1, 1)
end

function ddc.selection(room, entity)
    if entity.type == "Rectangle" then
        return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
    elseif entity.type == "Line" then
        local node = entity.nodes[1]
        local nodeRectangle = utils.rectangle(node.x - 8, node.y - 8, 16, 16)
        return utils.rectangle(entity.x - 8, entity.y - 8, 16, 16), { nodeRectangle }
    elseif entity.type == "Text" then
        return utils.rectangle(entity.x - 16, entity.y - 16, 32, 32)
    else
        return utils.rectangle(entity.x - 8, entity.y - 8, 16, 16)
    end
end

ddc.nodeVisibility = "always"
ddc.nodeLineRenderType = "none"

return ddc
