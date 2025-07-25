local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")
local ddc = {}

ddc.name = "GameHelper/DebugDecalController"
ddc.justification = {0.5, 0.5}
ddc.nodeLineRenderType = "line"
ddc.placements = {
    {
        name = "image",
        data = {
            type = "Image",
            sprite = "loenn/GameHelper/debug_decal_controller",
            scaleX = 1.0,
            scaleY = 1.0,
            color = "FFFFFF",
			width = 16,
			height = 16
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
            color = "FFFFFF",
			width = 8,
			height = 8
        }
    },
    {
        name = "text",
        data = {
            type = "Text",
            dialog = "Hello",
            color = "FFFFFF",
            scale = 1.0,
			width = 16,
			height = 8
        }
    }
}

ddc.fieldInformation = {
    color = { fieldType = "color" },
}

ddc.ignoredFields = {
    "_id", "_name", "originX", "originY", "type", "width", "height"
}

function ddc.depth(room, entity)
	if entity.type == "Rectangle" then
		return -1
	else
		return -99999
	end
end

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
    local color = {0.411, 0.411, 0.933}
    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)
        if success then
            color = {r, g, b}
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

    local colorHex = entity.color or ddc.color
    local r, g, b = hexToRGB(colorHex)
    love.graphics.setColor(r, g, b)
    
    if entity.type  == "Image" then
        local debugImage = drawableSprite.fromTexture(entity.sprite, entity)
		debugImage:addPosition(8,8)
        if debugImage ~= nil then
            debugImage:draw()
        end
    else
		if entity.type == "Rectangle" then
			if entity.hollow == true then
				love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)
			else
				love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
			end
		elseif entity.type == "Text" then
			if entity.dialog ~= "" then
				love.graphics.print(entity.dialog, entity.x, entity.y)
			else
				love.graphics.print("Text", entity.x, entity.y)
			end
		elseif entity.type == "Line" then
			love.graphics.rectangle("fill", entity.x, entity.y, 4, 4)
			if nodes and nodes[1] then
				love.graphics.rectangle("fill", nodes[1].x, nodes[1].y, 4, 4)
				love.graphics.line(entity.x + 2, entity.y + 2, nodes[1].x + 2, nodes[1].y + 2)
			end
        end
    end
    love.graphics.setColor(1, 1, 1)
end

function ddc.nodeSprite(room, entity)
	return "loenn/GameHelper/debug_decal_controller"
end

ddc.nodeVisibility = "always"
ddc.nodeLineRenderType = "line"

function ddc.nodeRectangle(room, entity, node, nodeIndex, viewport)
    return utils.rectangle(node.x, node.y, entity.width, entity.height)
end

--function ddc.scale(room, entity)
--    return { entity.scaleX or 1, entity.scaleY or 1 }
--end

return ddc