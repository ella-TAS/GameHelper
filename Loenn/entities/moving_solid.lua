local utils = require("utils")
local fakeTilesHelper = require("helpers.fake_tiles")
local easingOptions = require("mods").requireFromPlugin("easing_options")

local solid = {}

solid.name = "GameHelper/MovingSolid"
solid.depth = 8998
solid.canResize = { true, true }
solid.nodeLimits = { 1, 1 }
solid.nodeLineRenderType = "line"
solid.nodeVisibility = "always"
solid.placements = {
    name = "normal",
    data = {
        width = 8,
        height = 8,
        tileset = "3",
        moveTime = 2.0,
        easeMode = "CubeInOut",
        startOffset = 0.0,
        pauseTime = 0.0
    }
}

solid.sprite = fakeTilesHelper.getEntitySpriteFunction("tileset")

function solid.fieldInformation(entity)
    local orig = fakeTilesHelper.getFieldInformation("tileset")(entity)
    orig.easeMode = {
        options = easingOptions,
        editable = false
    }
    return orig
end

--take out on Lönn fix
function solid.nodeRectangle(room, entity, node, nodeIndex, viewport)
    return utils.rectangle(node.x, node.y, entity.width, entity.height)
end

return solid
