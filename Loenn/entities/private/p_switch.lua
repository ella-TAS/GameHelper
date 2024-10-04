local pswitch = {}

local spriteOptions = {"normal", "blue"}

pswitch.name = "GameHelper/PSwitch"
pswitch.depth = 8998
pswitch.justification = {0.5, 1.0}
pswitch.placements = {
    name = "normal",
    data = {
        showTutorial = false,
        stationary = false,
        flag = "pswitch",
        flagDuration = 7,
        sprite = "normal"
    }
}
pswitch.fieldInformation = {
    sprite = {
        options = spriteOptions,
        editable = false
    }
}

function pswitch.texture(room, entity)
    return "objects/GameHelper/p_switch/" .. (entity.sprite or "blue") .. "/normal00"
end

return pswitch