local utils = require("utils")
local fakeTilesHelper = require("helpers.fake_tiles")
local drawableSprite = require("structs.drawable_sprite")

local solid = {}

solid.name = "GameHelper/PSwitchBlock"
solid.depth = 8998
solid.canResize = {true, true}
solid.justification = {0.0, 0.0}
solid.placements = {
    name = "normal",
    data = {
        width = 8,
        height = 8,
        tiletype = "3",
        blendin = false,
        permanent = false,
        canDash = true,
        flag = "pswitch",
        startAsBlock = true
    }
}

function solid.sprite(room, entity, viewport)
    return entity.startAsBlock and fakeTilesHelper.getEntitySpriteFunction("tiletype")(room, entity) or drawableSprite.fromTexture("objects/GameHelper/p_switch/coin00", entity)
end

solid.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return solid