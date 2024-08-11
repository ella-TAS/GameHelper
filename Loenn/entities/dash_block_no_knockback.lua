local utils = require("utils")
local fakeTilesHelper = require("helpers.fake_tiles")

local solid = {}

solid.name = "GameHelper/DashBlockNoKnockback"
solid.depth = 8998
solid.canResize = {true, true}
solid.placements = {
    name = "normal",
    data = {
        width = 8,
        height = 8,
        tileset = "3",
        blendin = false,
        permanent = false,
        canDash = true
    }
}

solid.sprite = fakeTilesHelper.getEntitySpriteFunction("tileset")

solid.fieldInformation = function(entity)
    return fakeTilesHelper.getFieldInformation("tileset")(entity)
end

return solid