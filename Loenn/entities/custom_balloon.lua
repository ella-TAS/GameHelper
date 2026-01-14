local utils = require("utils")

local balloon = {}

balloon.name = "GameHelper/CustomBalloon"
balloon.depth = 8998
balloon.justification = { 0.0, 0.18 }
balloon.fieldInformation = {
    
}
balloon.placements = {
    name = "balloon",
    data = {
        oneUse = false,
        speedYBounce = 140,
        multiplySpeed = true,
        speedXModifier = 1.2,
        refillDash = true,
        refillDoubleDash = false,
        refillStamina = true,
        floaty = true,
        popAudio = "event:/GameHelper/balloon/Balloon_pop",
        hitboxData = "15,8,0,0",
        depth = -1,
        spriteXmlPath = "red"
    }
}

function balloon.rectangle(room, entity, viewport)
    return utils.rectangle(entity.x, entity.y - 4, 15, 17)
end

function balloon.texture(room, entity)
    if entity.spriteXmlPath == "blue" or entity.spriteXmlPath == "green" or entity.spriteXmlPath == "yellow" then
        return "loenn/GameHelper/balloon_" .. entity.spriteXmlPath
    else
        return "loenn/GameHelper/balloon_red"
    end
end

return balloon
