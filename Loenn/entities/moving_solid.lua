local fakeTilesHelper = require("helpers.fake_tiles")
local easingOptions = require("mods").requireFromPlugin("GHoptions")

local solid = {}

local function tableConcat(t1,t2)
    for i=1,#t2 do
        t1[#t1+1] = t2[i]
    end
    return t1
end

solid.name = "GameHelper/MovingSolid"
solid.depth = 8998
solid.canResize = {true, true}
solid.nodeLimits = {1, -1}
solid.placements = {
    name = "normal",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        moveTime = 2.0,
        easeMode = "CubeInOut"
    }
}
solid.fieldInformation = {
    easeMode = {
        options = easingOptions,
        editable = false
    }
}
solid.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")
solid.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype")

return solid