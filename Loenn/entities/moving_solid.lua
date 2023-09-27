local fakeTilesHelper = require("helpers.fake_tiles")
local easingOptions = require("mods").requireFromPlugin("GHoptions")

local solid = {}

solid.name = "GameHelper/MovingSolid"
solid.depth = 8998
solid.canResize = {true, true}
solid.nodeLimits = {1, 1}
solid.nodeLineRenderType = "line"
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
solid.fieldInformation = {
    easeMode = {
        options = easingOptions,
        editable = false
    }
}
solid.sprite = fakeTilesHelper.getEntitySpriteFunction("tileset")

solid.fieldInformation = function(entity)
    local orig = fakeTilesHelper.getFieldInformation("tileset")(entity)
    orig.easeMode = {
        options = easingOptions,
        editable = false
    }
    return orig
end

return solid