local fakeTilesHelper = require("helpers.fake_tiles")
local tdd = {}

tdd.name = "GameHelper/TileDebugDecalConverter"
tdd.depth = -999999999
tdd.texture = "loenn/GameHelper/colorful_debug_controller"
tdd.justification = { 0.0, 0.0 }
tdd.placements = {
    {
        name = "fg",
        data = {
            fg = true,
            tileset = fakeTilesHelper.getPlacementMaterial(),
            color = "FFFFFF"
        }
    },
    {
        name = "bg",
        data = {
            fg = false,
            tileset = "1",
            color = "2F4F4F"
        }
    }
}

function tdd.fieldInformation(entity)
    local orig = fakeTilesHelper.getFieldInformation("tileset", entity.fg and "tilesFg" or "tilesBg")(entity)
    orig.color = { fieldType = "color" }
    return orig
end

tdd.ignoredFields = {
    "_id", "_name", "originX", "originY", "fg"
}

return tdd
